import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:spotfinder_app/features/explore/data/models/concept_tag_model.dart';
import 'package:spotfinder_app/features/explore/data/models/district_model.dart';
import 'package:spotfinder_app/features/explore/presentation/bloc/search_bloc.dart';

class ExploreScreen extends StatefulWidget {
  const ExploreScreen({super.key});

  @override
  State<ExploreScreen> createState() => _ExploreScreenState();
}

class _ExploreScreenState extends State<ExploreScreen> {
  int? _selectedDistrictId;
  final Set<int> _selectedTagIds = {};
  String _sortBy = 'rating';
  final TextEditingController _nameController = TextEditingController();

  // State geçişlerinde kaybolmaması için district/tag listelerini cache'le
  List<DistrictModel> _cachedDistricts = [];
  List<ConceptTagModel> _cachedTags = [];

  @override
  void dispose() {
    _nameController.dispose();
    super.dispose();
  }

  @override
  void initState() {
    super.initState();
    final state = context.read<SearchBloc>().state;
    if (state is FiltersLoaded) {
      _cachedDistricts = state.districts;
      _cachedTags = state.tags;
      _selectedDistrictId = state.selectedDistrictId;
      _selectedTagIds.addAll(state.selectedTagIds);
      _sortBy = state.sortBy;
    } else {
      context.read<SearchBloc>().add(const LoadFilters());
    }
  }

  void _applyFilters() {
    context.read<SearchBloc>().add(FilterChanged(
          districtId: _selectedDistrictId,
          tagIds: _selectedTagIds.toList(),
          sortBy: _sortBy,
        ));
    final nameQuery = _nameController.text.trim().isEmpty ? null : _nameController.text.trim();
    context.read<SearchBloc>().add(SearchVenues(nameQuery: nameQuery));
    context.push('/search');
  }

  void _clearFilters() {
    setState(() {
      _selectedDistrictId = null;
      _selectedTagIds.clear();
      _sortBy = 'rating';
      _nameController.clear();
    });
  }

  void _showConceptTagBottomSheet(BuildContext context, List<ConceptTagModel> tags) {
    final theme = Theme.of(context);
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (ctx) => StatefulBuilder(
        builder: (ctx, setModalState) {
          return DraggableScrollableSheet(
            initialChildSize: 0.6,
            maxChildSize: 0.9,
            minChildSize: 0.4,
            expand: false,
            builder: (_, scrollController) => Column(
              children: [
                Padding(
                  padding: const EdgeInsets.fromLTRB(20, 16, 8, 8),
                  child: Row(
                    children: [
                      Text('Konsept Seç',
                          style: theme.textTheme.titleMedium
                              ?.copyWith(fontWeight: FontWeight.w600)),
                      const Spacer(),
                      TextButton(
                        onPressed: () {
                          setState(() => _selectedTagIds.clear());
                          setModalState(() {});
                        },
                        child: const Text('Temizle'),
                      ),
                    ],
                  ),
                ),
                const Divider(height: 1),
                Expanded(
                  child: ListView(
                    controller: scrollController,
                    children: tags.map((tag) {
                      final selected = _selectedTagIds.contains(tag.id);
                      return CheckboxListTile(
                        title: Text(tag.nameTr),
                        value: selected,
                        onChanged: (v) {
                          setState(() {
                            if (v == true) {
                              _selectedTagIds.add(tag.id);
                            } else {
                              _selectedTagIds.remove(tag.id);
                            }
                          });
                          setModalState(() {});
                        },
                      );
                    }).toList(),
                  ),
                ),
                Padding(
                  padding: const EdgeInsets.all(16),
                  child: SizedBox(
                    width: double.infinity,
                    child: FilledButton(
                      onPressed: () => Navigator.pop(ctx),
                      child: Text(_selectedTagIds.isEmpty
                          ? 'Tamam'
                          : 'Tamam (${_selectedTagIds.length} seçili)'),
                    ),
                  ),
                ),
              ],
            ),
          );
        },
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Keşfet & Filtrele'),
        actions: [
          TextButton(
            onPressed: _clearFilters,
            child: const Text('Temizle'),
          ),
        ],
      ),
      body: BlocListener<SearchBloc, SearchState>(
        listener: (context, state) {
          if (state is FiltersLoaded) {
            setState(() {
              _cachedDistricts = state.districts;
              _cachedTags = state.tags;
            });
          }
        },
        child: BlocBuilder<SearchBloc, SearchState>(
          buildWhen: (prev, curr) => curr is FiltersLoaded || curr is FiltersLoading || curr is SearchError,
          builder: (context, state) {
            if (state is FiltersLoading && _cachedDistricts.isEmpty) {
              return const Center(child: CircularProgressIndicator());
            }
            if (state is SearchError && _cachedDistricts.isEmpty) {
              return Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Text(state.message),
                    const SizedBox(height: 12),
                    ElevatedButton(
                      onPressed: () => context.read<SearchBloc>().add(const LoadFilters()),
                      child: const Text('Tekrar Dene'),
                    ),
                  ],
                ),
              );
            }

            // Cache'lenmiş veya anlık yüklü district/tag listesini kullan
            final districts = _cachedDistricts;
            final tags = _cachedTags;

            // Seçili ilçe artık listede yoksa null'a düşür (defensive guard)
            final safeDistrictId = districts.any((d) => d.id == _selectedDistrictId)
                ? _selectedDistrictId
                : null;

            return SingleChildScrollView(
              padding: const EdgeInsets.all(20),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // ─── Mekan Adı Araması ────────────────────────────────────
                  Text(
                    'Mekân Adı',
                    style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w600),
                  ),
                  const SizedBox(height: 12),
                  TextField(
                    controller: _nameController,
                    decoration: InputDecoration(
                      hintText: 'Mekân adı ile ara...',
                      prefixIcon: const Icon(Icons.search),
                      border: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(12),
                        borderSide: BorderSide(color: colorScheme.outline.withOpacity(0.4)),
                      ),
                      enabledBorder: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(12),
                        borderSide: BorderSide(color: colorScheme.outline.withOpacity(0.4)),
                      ),
                      contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
                    ),
                    onSubmitted: (_) => _applyFilters(),
                  ),

                  const SizedBox(height: 28),

                  // ─── İlçe Seçimi ─────────────────────────────────────────
                  Text(
                    'İlçe',
                    style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w600),
                  ),
                  const SizedBox(height: 12),
                  Container(
                    decoration: BoxDecoration(
                      border: Border.all(color: colorScheme.outline.withOpacity(0.4)),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: DropdownButtonHideUnderline(
                      child: DropdownButton<int?>(
                        value: safeDistrictId,
                        isExpanded: true,
                        padding: const EdgeInsets.symmetric(horizontal: 16),
                        borderRadius: BorderRadius.circular(12),
                        hint: const Text('Tüm İlçeler'),
                        items: [
                          const DropdownMenuItem<int?>(
                            value: null,
                            child: Text('Tüm İlçeler'),
                          ),
                          ...districts.map(
                            (d) => DropdownMenuItem<int?>(
                              value: d.id,
                              child: Text(d.name),
                            ),
                          ),
                        ],
                        onChanged: (v) => setState(() => _selectedDistrictId = v),
                      ),
                    ),
                  ),

                  const SizedBox(height: 28),

                  // ─── Konsept Etiketleri ───────────────────────────────────
                  Text(
                    'Konsept',
                    style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w600),
                  ),
                  const SizedBox(height: 12),
                  GestureDetector(
                    onTap: tags.isEmpty ? null : () => _showConceptTagBottomSheet(context, tags),
                    child: Container(
                      decoration: BoxDecoration(
                        border: Border.all(color: colorScheme.outline.withOpacity(0.4)),
                        borderRadius: BorderRadius.circular(12),
                      ),
                      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
                      child: Row(
                        children: [
                          Expanded(
                            child: Text(
                              tags.isEmpty
                                  ? 'Konseptler yükleniyor...'
                                  : _selectedTagIds.isEmpty
                                      ? 'Tüm Konseptler'
                                      : '${_selectedTagIds.length} konsept seçili',
                              style: theme.textTheme.bodyLarge?.copyWith(
                                color: _selectedTagIds.isEmpty
                                    ? colorScheme.onSurfaceVariant
                                    : colorScheme.onSurface,
                              ),
                            ),
                          ),
                          Icon(Icons.keyboard_arrow_down,
                              color: colorScheme.onSurfaceVariant),
                        ],
                      ),
                    ),
                  ),

                  const SizedBox(height: 28),

                  // ─── Sıralama ─────────────────────────────────────────────
                  Text(
                    'Sıralama',
                    style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w600),
                  ),
                  const SizedBox(height: 12),
                  Row(
                    children: [
                      _SortChip(
                        label: 'Puana Göre',
                        value: 'rating',
                        selected: _sortBy == 'rating',
                        onTap: () => setState(() => _sortBy = 'rating'),
                      ),
                      const SizedBox(width: 10),
                      _SortChip(
                        label: 'En Yeni',
                        value: 'newest',
                        selected: _sortBy == 'newest',
                        onTap: () => setState(() => _sortBy = 'newest'),
                      ),
                    ],
                  ),

                  const SizedBox(height: 40),

                  // ─── Ara Butonu ───────────────────────────────────────────
                  SizedBox(
                    width: double.infinity,
                    child: ElevatedButton.icon(
                      onPressed: _applyFilters,
                      icon: const Icon(Icons.search),
                      label: const Text('Mekânları Ara'),
                      style: ElevatedButton.styleFrom(
                        padding: const EdgeInsets.symmetric(vertical: 14),
                      ),
                    ),
                  ),
                ],
              ),
            );
          },
        ),
      ),
    );
  }
}

class _SortChip extends StatelessWidget {
  final String label;
  final String value;
  final bool selected;
  final VoidCallback onTap;

  const _SortChip({
    required this.label,
    required this.value,
    required this.selected,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;
    return GestureDetector(
      onTap: onTap,
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
        decoration: BoxDecoration(
          color: selected ? colorScheme.primary : Colors.transparent,
          borderRadius: BorderRadius.circular(10),
          border: Border.all(
            color: selected ? colorScheme.primary : colorScheme.outline.withOpacity(0.5),
          ),
        ),
        child: Text(
          label,
          style: TextStyle(
            fontSize: 14,
            fontWeight: selected ? FontWeight.w600 : FontWeight.w400,
            color: selected ? colorScheme.onPrimary : colorScheme.onSurface,
          ),
        ),
      ),
    );
  }
}
