import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:spotfinder_app/core/di/service_locator.dart';
import 'package:spotfinder_app/features/reviews/presentation/bloc/review_bloc.dart';

class WriteReviewScreen extends StatefulWidget {
  final String venueId;
  final String venueName;

  const WriteReviewScreen({
    super.key,
    required this.venueId,
    required this.venueName,
  });

  @override
  State<WriteReviewScreen> createState() => _WriteReviewScreenState();
}

class _WriteReviewScreenState extends State<WriteReviewScreen> {
  int _rating = 0;
  final _bodyController = TextEditingController();
  final _formKey = GlobalKey<FormState>();

  @override
  void dispose() {
    _bodyController.dispose();
    super.dispose();
  }

  void _submit(ReviewBloc bloc) {
    if (_rating == 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Lütfen bir puan seçin.')),
      );
      return;
    }
    if (!_formKey.currentState!.validate()) return;

    bloc.add(SubmitReview(
      venueId: widget.venueId,
      body: _bodyController.text.trim(),
      rating: _rating,
    ));
  }

  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (_) => ReviewBloc(reviewRepository: ServiceLocator.reviewRepository),
      child: Builder(
        builder: (context) => BlocListener<ReviewBloc, ReviewState>(
          listener: (context, state) {
            if (state is ReviewSubmitted) {
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(content: Text('Yorumunuz gönderildi.')),
              );
              context.pop();
            } else if (state is ReviewError) {
              ScaffoldMessenger.of(context).showSnackBar(
                SnackBar(content: Text(state.message)),
              );
            }
          },
          child: Scaffold(
            appBar: AppBar(title: const Text('Yorum Yaz')),
            body: SingleChildScrollView(
              padding: const EdgeInsets.all(20),
              child: Form(
                key: _formKey,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      widget.venueName,
                      style: Theme.of(context).textTheme.titleLarge?.copyWith(
                            fontWeight: FontWeight.w600,
                          ),
                    ),
                    const SizedBox(height: 24),
                    Text(
                      'Puanınız',
                      style: Theme.of(context).textTheme.titleSmall,
                    ),
                    const SizedBox(height: 8),
                    Row(
                      children: List.generate(5, (index) {
                        final star = index + 1;
                        return GestureDetector(
                          onTap: () => setState(() => _rating = star),
                          child: Padding(
                            padding: const EdgeInsets.only(right: 8),
                            child: Icon(
                              star <= _rating ? Icons.star : Icons.star_border,
                              size: 36,
                              color: star <= _rating
                                  ? Colors.amber
                                  : Theme.of(context).colorScheme.outline,
                            ),
                          ),
                        );
                      }),
                    ),
                    const SizedBox(height: 24),
                    Text(
                      'Yorumunuz',
                      style: Theme.of(context).textTheme.titleSmall,
                    ),
                    const SizedBox(height: 8),
                    TextFormField(
                      controller: _bodyController,
                      maxLines: 5,
                      maxLength: 500,
                      decoration: const InputDecoration(
                        hintText: 'Bu mekânla ilgili düşüncelerinizi yazın...',
                        border: OutlineInputBorder(),
                      ),
                      validator: (v) {
                        if (v == null || v.trim().isEmpty) {
                          return 'Yorum boş bırakılamaz.';
                        }
                        if (v.trim().length < 10) {
                          return 'Yorum en az 10 karakter olmalıdır.';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: 24),
                    BlocBuilder<ReviewBloc, ReviewState>(
                      builder: (context, state) {
                        final isSubmitting = state is ReviewSubmitting;
                        return SizedBox(
                          width: double.infinity,
                          child: FilledButton(
                            onPressed: isSubmitting
                                ? null
                                : () => _submit(context.read<ReviewBloc>()),
                            child: isSubmitting
                                ? const SizedBox(
                                    height: 20,
                                    width: 20,
                                    child: CircularProgressIndicator(
                                      strokeWidth: 2,
                                      color: Colors.white,
                                    ),
                                  )
                                : const Text('Gönder'),
                          ),
                        );
                      },
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
