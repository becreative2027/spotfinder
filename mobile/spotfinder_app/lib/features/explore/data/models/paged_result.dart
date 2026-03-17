class PagedResult<T> {
  final List<T> items;
  final int totalCount;
  final int page;
  final int pageSize;
  final int totalPages;

  const PagedResult({
    required this.items,
    required this.totalCount,
    required this.page,
    required this.pageSize,
    required this.totalPages,
  });

  factory PagedResult.fromJson(
    Map<String, dynamic> json,
    T Function(Map<String, dynamic>) fromJsonItem,
  ) =>
      PagedResult(
        items: (json['items'] as List<dynamic>)
            .map((e) => fromJsonItem(e as Map<String, dynamic>))
            .toList(),
        totalCount: json['totalCount'] as int? ?? 0,
        page: json['page'] as int? ?? 1,
        pageSize: json['pageSize'] as int? ?? 20,
        totalPages: json['totalPages'] as int? ?? 1,
      );

  bool get hasNextPage => page < totalPages;
}
