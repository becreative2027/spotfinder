'use client';

import { useEffect, useState, FormEvent } from 'react';
import AdminLayout from '@/components/AdminLayout';
import LoadingSpinner from '@/components/LoadingSpinner';
import { getConceptTags, createConceptTag, deleteConceptTag, ConceptTag } from '@/lib/api';

export default function ConceptTagsPage() {
  const [tags, setTags] = useState<ConceptTag[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [nameTr, setNameTr] = useState('');
  const [nameEn, setNameEn] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [formError, setFormError] = useState('');

  async function fetchTags() {
    setLoading(true);
    setError('');
    try {
      const data = await getConceptTags();
      setTags(data);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Etiketler alınamadı.';
      setError(message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    fetchTags();
  }, []);

  async function handleCreate(e: FormEvent) {
    e.preventDefault();
    setFormError('');
    setSubmitting(true);
    try {
      const newTag = await createConceptTag(nameTr, nameEn);
      setTags((prev) => [...prev, newTag]);
      setNameTr('');
      setNameEn('');
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Etiket oluşturulamadı.';
      setFormError(message);
    } finally {
      setSubmitting(false);
    }
  }

  async function handleDelete(id: string, name: string) {
    if (!window.confirm(`"${name}" etiketini silmek istediğinize emin misiniz?`)) return;
    try {
      await deleteConceptTag(id);
      setTags((prev) => prev.filter((t) => t.id !== id));
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Etiket silinemedi.';
      alert(message);
    }
  }

  return (
    <AdminLayout title="Konsept Etiketleri">
      <div className="max-w-3xl space-y-6">
        {/* Add Tag Form */}
        <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-6">
          <h2 className="text-base font-semibold text-gray-800 mb-4">Yeni Etiket Ekle</h2>

          {formError && (
            <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
              {formError}
            </div>
          )}

          <form onSubmit={handleCreate} className="flex flex-col sm:flex-row gap-3">
            <div className="flex-1">
              <label className="block text-xs font-medium text-gray-600 mb-1">Türkçe Ad</label>
              <input
                type="text"
                required
                value={nameTr}
                onChange={(e) => setNameTr(e.target.value)}
                placeholder="örn. Romantik"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
              />
            </div>
            <div className="flex-1">
              <label className="block text-xs font-medium text-gray-600 mb-1">İngilizce Ad</label>
              <input
                type="text"
                required
                value={nameEn}
                onChange={(e) => setNameEn(e.target.value)}
                placeholder="örn. Romantic"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
              />
            </div>
            <div className="flex items-end">
              <button
                type="submit"
                disabled={submitting}
                className="px-5 py-2 bg-indigo-600 hover:bg-indigo-700 disabled:bg-indigo-400 text-white text-sm font-semibold rounded-lg transition-colors whitespace-nowrap"
              >
                {submitting ? 'Ekleniyor...' : 'Ekle'}
              </button>
            </div>
          </form>
        </div>

        {/* Tags Table */}
        {error && (
          <div className="p-4 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
            {error}
          </div>
        )}

        {loading ? (
          <LoadingSpinner />
        ) : (
          <div className="bg-white rounded-xl border border-gray-200 shadow-sm overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-100">
              <p className="text-sm text-gray-500">
                Toplam <span className="font-semibold text-gray-700">{tags.length}</span> etiket
              </p>
            </div>
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">ID</th>
                    <th className="px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Türkçe Ad</th>
                    <th className="px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">İngilizce Ad</th>
                    <th className="px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Sistem?</th>
                    <th className="px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider">Aktif?</th>
                    <th className="px-4 py-3 text-right text-xs font-semibold text-gray-500 uppercase tracking-wider">İşlemler</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100">
                  {tags.length === 0 ? (
                    <tr>
                      <td colSpan={6} className="text-center py-12 text-gray-400 text-sm">
                        Henüz etiket bulunmuyor.
                      </td>
                    </tr>
                  ) : (
                    tags.map((tag) => (
                      <tr key={tag.id} className="hover:bg-gray-50 transition-colors">
                        <td className="px-4 py-3 text-xs font-mono text-gray-400 max-w-xs">
                          <span className="truncate block" title={tag.id}>
                            {tag.id.length > 8 ? `${tag.id.slice(0, 8)}...` : tag.id}
                          </span>
                        </td>
                        <td className="px-4 py-3 text-sm font-medium text-gray-900">{tag.nameTr}</td>
                        <td className="px-4 py-3 text-sm text-gray-600">{tag.nameEn}</td>
                        <td className="px-4 py-3">
                          <span
                            className={`inline-flex px-2 py-0.5 rounded-full text-xs font-medium ${
                              tag.isSystem
                                ? 'bg-blue-100 text-blue-700'
                                : 'bg-gray-100 text-gray-500'
                            }`}
                          >
                            {tag.isSystem ? 'Evet' : 'Hayır'}
                          </span>
                        </td>
                        <td className="px-4 py-3">
                          <span
                            className={`inline-flex px-2 py-0.5 rounded-full text-xs font-medium ${
                              tag.isActive
                                ? 'bg-green-100 text-green-700'
                                : 'bg-gray-100 text-gray-500'
                            }`}
                          >
                            {tag.isActive ? 'Aktif' : 'Pasif'}
                          </span>
                        </td>
                        <td className="px-4 py-3 text-right">
                          <button
                            onClick={() => handleDelete(tag.id, tag.nameTr)}
                            className="px-3 py-1.5 text-xs font-medium text-red-600 hover:text-red-800 bg-red-50 hover:bg-red-100 rounded-md transition-colors"
                          >
                            Sil
                          </button>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>
    </AdminLayout>
  );
}
