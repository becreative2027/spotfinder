'use client';

import { useEffect, useState, FormEvent } from 'react';
import { useRouter } from 'next/navigation';
import AdminLayout from '@/components/AdminLayout';
import LoadingSpinner from '@/components/LoadingSpinner';
import { getDistricts, getConceptTags, createVenue, District, ConceptTag } from '@/lib/api';

const PARKING_OPTIONS = [
  { value: '', label: 'Seçiniz...' },
  { value: 'available', label: 'Mevcut' },
  { value: 'unavailable', label: 'Mevcut Değil' },
  { value: 'valet', label: 'Vale' },
  { value: 'empty', label: 'Boş Alan' },
];

export default function NewVenuePage() {
  const router = useRouter();
  const [districts, setDistricts] = useState<District[]>([]);
  const [conceptTags, setConceptTags] = useState<ConceptTag[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [districtId, setDistrictId] = useState('');
  const [address, setAddress] = useState('');
  const [parkingStatus, setParkingStatus] = useState('');
  const [lat, setLat] = useState('');
  const [lng, setLng] = useState('');
  const [selectedTagIds, setSelectedTagIds] = useState<string[]>([]);

  useEffect(() => {
    async function fetchData() {
      try {
        const [districtsData, tagsData] = await Promise.all([
          getDistricts(),
          getConceptTags(),
        ]);
        setDistricts(districtsData);
        setConceptTags(tagsData);
      } catch (err: unknown) {
        const message = err instanceof Error ? err.message : 'Veriler alınamadı.';
        setError(message);
      } finally {
        setLoading(false);
      }
    }
    fetchData();
  }, []);

  function toggleTag(id: string) {
    setSelectedTagIds((prev) =>
      prev.includes(id) ? prev.filter((t) => t !== id) : [...prev, id]
    );
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError('');
    setSubmitting(true);
    try {
      await createVenue({
        name,
        description: description || undefined,
        districtId,
        address,
        parkingStatus,
        lat: parseFloat(lat),
        lng: parseFloat(lng),
        conceptTagIds: selectedTagIds,
      });
      router.push('/venues');
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Mekân oluşturulamadı.';
      setError(message);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <AdminLayout title="Yeni Mekân Ekle">
      {loading ? (
        <LoadingSpinner />
      ) : (
        <div className="max-w-2xl">
          {error && (
            <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
              {error}
            </div>
          )}

          <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-6">
            <form onSubmit={handleSubmit} className="space-y-5">
              {/* Name */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Mekân Adı <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  required
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  placeholder="Mekân adını girin"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                />
              </div>

              {/* Description */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Açıklama</label>
                <textarea
                  rows={3}
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                  placeholder="Mekân hakkında kısa açıklama"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent resize-none"
                />
              </div>

              {/* District */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  İlçe <span className="text-red-500">*</span>
                </label>
                <select
                  required
                  value={districtId}
                  onChange={(e) => setDistrictId(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                >
                  <option value="">İlçe seçiniz...</option>
                  {districts.map((d) => (
                    <option key={d.id} value={d.id}>
                      {d.name} - {d.city}
                    </option>
                  ))}
                </select>
              </div>

              {/* Address */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Adres <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  required
                  value={address}
                  onChange={(e) => setAddress(e.target.value)}
                  placeholder="Tam adres"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                />
              </div>

              {/* Parking Status */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Otopark Durumu <span className="text-red-500">*</span>
                </label>
                <select
                  required
                  value={parkingStatus}
                  onChange={(e) => setParkingStatus(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                >
                  {PARKING_OPTIONS.map((o) => (
                    <option key={o.value} value={o.value} disabled={o.value === ''}>
                      {o.label}
                    </option>
                  ))}
                </select>
              </div>

              {/* Lat / Lng */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Enlem (Lat) <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="number"
                    step="any"
                    required
                    value={lat}
                    onChange={(e) => setLat(e.target.value)}
                    placeholder="41.0082"
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Boylam (Lng) <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="number"
                    step="any"
                    required
                    value={lng}
                    onChange={(e) => setLng(e.target.value)}
                    placeholder="28.9784"
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                  />
                </div>
              </div>

              {/* Concept Tags */}
              {conceptTags.length > 0 && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Konsept Etiketleri</label>
                  <div className="flex flex-wrap gap-2">
                    {conceptTags.map((tag) => (
                      <label
                        key={tag.id}
                        className={`flex items-center gap-1.5 px-3 py-1.5 rounded-full text-xs font-medium cursor-pointer border transition-colors ${
                          selectedTagIds.includes(tag.id)
                            ? 'bg-indigo-600 text-white border-indigo-600'
                            : 'bg-white text-gray-600 border-gray-300 hover:border-indigo-400'
                        }`}
                      >
                        <input
                          type="checkbox"
                          className="sr-only"
                          checked={selectedTagIds.includes(tag.id)}
                          onChange={() => toggleTag(tag.id)}
                        />
                        {tag.nameTr}
                      </label>
                    ))}
                  </div>
                </div>
              )}

              {/* Actions */}
              <div className="flex items-center gap-3 pt-2">
                <button
                  type="submit"
                  disabled={submitting}
                  className="px-5 py-2 bg-indigo-600 hover:bg-indigo-700 disabled:bg-indigo-400 text-white text-sm font-semibold rounded-lg transition-colors"
                >
                  {submitting ? 'Kaydediliyor...' : 'Mekânı Oluştur'}
                </button>
                <button
                  type="button"
                  onClick={() => router.push('/venues')}
                  className="px-5 py-2 bg-white hover:bg-gray-50 text-gray-700 text-sm font-semibold rounded-lg border border-gray-300 transition-colors"
                >
                  İptal
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </AdminLayout>
  );
}
