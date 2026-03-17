import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:spotfinder_app/features/explore/data/models/concept_tag_model.dart';
import 'package:spotfinder_app/features/explore/data/models/district_model.dart';
import 'package:spotfinder_app/features/explore/presentation/bloc/search_bloc.dart';
import 'package:spotfinder_app/shared/widgets/concept_tag_chip.dart';

class ExploreScreen extends StatefulWidget {
  const ExploreScreen({super.key});

  @override
  State<ExploreScreen> createState() => _ExploreScreenState();
}

class _ExploreScreenState extends State<ExploreScreen> {
  int? _selectedDistrictId;
  final Set<int> _selectedTagIds = {};
  String _sortBy = 'rating';

  @override
  void initState() {
    super.initState();
    final state = context.read<SearchBloc>().state;
    if (state is! FiltersLoaded) {
      context.read<SearchBloc>().add(const LoadFilters());
    } else {
      _selectedDistrictId = state.selectedDistrictId;
      _selectedTagIds.addAll(state.selectedTagIds);
      _sortBy = state.sortBy;
    }
  }

  void _applyFilters() {
    context.read<SearchBloc>().add(FilterChanged(
          districtId: _selectedDistrictId,
          tagIds: _selectedTagIds.toList(),
          sortBy: _sortBy,
        ));
    context.read<SearchBloc>().add(const SearchVenues());
    context.push('/search');
  }

  void _clearFilters() {
    setState(() {
      _selectedDistrictId = null;
      _selectedTagIds.clear();
      _sortBy = 'rating';
    });
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
      body: BlocConsumer<SearchBloc, SearchState>(
        listener: (context, state) {
          if (state is FiltersLoaded) {
            if (_selectedDistrictId == null && state.selectedDistrictId != null) {
              setState(() => _selectedDistrictId = state.selectedDistrictId);
            }
            if (_selectedTagIds.isEmpty && state.selectedTagIds.isNotEmpty) {
              setState(() => _selectedTagIds.addAll(state.selectedTagIds));
            }
          }
        },
        builder: (context, state) {
          if (state is FiltersLoading) {
            return const Center(child: CircularProgressIndicator());
          }
          if (state is SearchError) {
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

          final districts = state is FiltersLoaded ? state.districts : <DistrictModel>[];
          final tags = state is FiltersLoaded ? state.tags : <ConceptTagModel>[];

          return SingleChildScrollView(
            padding: const EdgeInsets.all(20),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
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
                      value: _selectedDistrictId,
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
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Konsept',
                      style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w600),
                    ),
                    if (_selectedTagIds.isNotEmpty)
                      Text(
                        '${_selectedTagIds.length} seçili',
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: colorScheme.primary,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                  ],
                ),
                const SizedBox(height: 12),
                Wrap(
                  spacing: 8,
                  runSpacing: 8,
                  children: tags.map((tag) {
                    final isSelected = _selectedTagIds.contains(tag.id);
                    return ConceptTagChip(
                      tag: tag,
                      isSelected: isSelected,
                      onTap: () {
                        setState(() {
                          if (isSelected) {
                            _selectedTagIds.remove(tag.id);
                          } else {
                            _selectedTagIds.add(tag.id);
                          }
                        });
                      },
                    );
                  }).toList(),
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
