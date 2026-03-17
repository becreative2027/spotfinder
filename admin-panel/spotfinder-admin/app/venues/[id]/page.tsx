'use client';

import { useEffect, useState, FormEvent } from 'react';
import { useParams, useRouter } from 'next/navigation';
import AdminLayout from '@/components/AdminLayout';
import LoadingSpinner from '@/components/LoadingSpinner';
import {
  getVenue,
  getDistricts,
  getConceptTags,
  updateVenue,
  addPhoto,
  deletePhoto,
  District,
  ConceptTag,
  Photo,
  Venue,
} from '@/lib/api';

const PARKING_OPTIONS = [
  { value: '', label: 'Seçiniz...' },
  { value: 'available', label: 'Mevcut' },
  { value: 'unavailable', label: 'Mevcut Değil' },
  { value: 'valet', label: 'Vale' },
  { value: 'empty', label: 'Boş Alan' },
];

export default function EditVenuePage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();

  const [venue, setVenue] = useState<Venue | null>(null);
  const [districts, setDistricts] = useState<District[]>([]);
  const [conceptTags, setConceptTags] = useState<ConceptTag[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [successMsg, setSuccessMsg] = useState('');

  // Form fields
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [districtId, setDistrictId] = useState('');
  const [address, setAddress] = useState('');
  const [parkingStatus, setParkingStatus] = useState('');
  const [lat, setLat] = useState('');
  const [lng, setLng] = useState('');
  const [selectedTagIds, setSelectedTagIds] = useState<string[]>([]);

  // Photos
  const [photos, setPhotos] = useState<Photo[]>([]);
  const [photoUrl, setPhotoUrl] = useState('');
  const [photoIsMenu, setPhotoIsMenu] = useState(false);
  const [photoOrder, setPhotoOrder] = useState('0');
  const [photoSubmitting, setPhotoSubmitting] = useState(false);
  const [photoError, setPhotoError] = useState('');

  useEffect(() => {
    async function fetchData() {
      try {
        const [venueData, districtsData, tagsData] = await Promise.all([
          getVenue(id),
          getDistricts(),
          getConceptTags(),
        ]);
        setVenue(venueData);
        setDistricts(districtsData);
        setConceptTags(tagsData);

        setName(venueData.name);
        setDescription(venueData.description || '');
        setDistrictId(venueData.districtId);
        setAddress(venueData.address);
        setParkingStatus(venueData.parkingStatus);
        setLat(String(venueData.lat));
        setLng(String(venueData.lng));
        setSelectedTagIds(venueData.conceptTags.map((t) => t.id));
        setPhotos(venueData.photos);
      } catch (err: unknown) {
        const message = err instanceof Error ? err.message : 'Veriler alınamadı.';
        setError(message);
      } finally {
        setLoading(false);
      }
    }
    fetchData();
  }, [id]);

  function toggleTag(tagId: string) {
    setSelectedTagIds((prev) =>
      prev.includes(tagId) ? prev.filter((t) => t !== tagId) : [...prev, tagId]
    );
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError('');
    setSuccessMsg('');
    setSubmitting(true);
    try {
      await updateVenue(id, {
        name,
        description: description || undefined,
        districtId,
        address,
        parkingStatus,
        lat: parseFloat(lat),
        lng: parseFloat(lng),
        conceptTagIds: selectedTagIds,
      });
      setSuccessMsg('Mekân başarıyla güncellendi.');
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Güncelleme başarısız.';
      setError(message);
    } finally {
      setSubmitting(false);
    }
  }

  async function handleAddPhoto(e: FormEvent) {
    e.preventDefault();
    setPhotoError('');
    setPhotoSubmitting(true);
    try {
      const newPhoto = await addPhoto(id, photoUrl, photoIsMenu, parseInt(photoOrder) || 0);
      setPhotos((prev) => [...prev, newPhoto]);
      setPhotoUrl('');
      setPhotoIsMenu(false);
      setPhotoOrder('0');
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Fotoğraf eklenemedi.';
      setPhotoError(message);
    } finally {
      setPhotoSubmitting(false);
    }
  }

  async function handleDeletePhoto(photoId: string) {
    if (!window.confirm('Bu fotoğrafı silmek istediğinize emin misiniz?')) return;
    try {
      await deletePhoto(id, photoId);
      setPhotos((prev) => prev.filter((p) => p.id !== photoId));
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Fotoğraf silinemedi.';
      alert(message);
    }
  }

  return (
    <AdminLayout title="Mekân Düzenle">
      {loading ? (
        <LoadingSpinner />
      ) : (
        <div className="max-w-2xl space-y-6">
          {error && (
            <div className="p-4 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
              {error}
            </div>
          )}
          {successMsg && (
            <div className="p-4 bg-green-50 border border-green-200 rounded-lg text-green-700 text-sm">
              {successMsg}
            </div>
          )}

          {/* Venue Form */}
          <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-6">
            <h2 className="text-base font-semibold text-gray-800 mb-5">Mekân Bilgileri</h2>
            <form onSubmit={handleSubmit} className="space-y-5">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Mekân Adı <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  required
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Açıklama</label>
                <textarea
                  rows={3}
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent resize-none"
                />
              </div>

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

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Adres <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  required
                  value={address}
                  onChange={(e) => setAddress(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                />
              </div>

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
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                  />
                </div>
              </div>

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

              <div className="flex items-center gap-3 pt-2">
                <button
                  type="submit"
                  disabled={submitting}
                  className="px-5 py-2 bg-indigo-600 hover:bg-indigo-700 disabled:bg-indigo-400 text-white text-sm font-semibold rounded-lg transition-colors"
                >
                  {submitting ? 'Güncelleniyor...' : 'Güncelle'}
                </button>
                <button
                  type="button"
                  onClick={() => router.push('/venues')}
                  className="px-5 py-2 bg-white hover:bg-gray-50 text-gray-700 text-sm font-semibold rounded-lg border border-gray-300 transition-colors"
                >
                  Geri Dön
                </button>
              </div>
            </form>
          </div>

          {/* Photos Section */}
          <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-6">
            <h2 className="text-base font-semibold text-gray-800 mb-4">Fotoğraflar</h2>

            {/* Existing photos */}
            {photos.length === 0 ? (
              <p className="text-sm text-gray-400 mb-4">Henüz fotoğraf eklenmemiş.</p>
            ) : (
              <div className="space-y-2 mb-5">
                {photos.map((photo) => (
                  <div
                    key={photo.id}
                    className="flex items-center gap-3 p-3 bg-gray-50 rounded-lg border border-gray-200"
                  >
                    {/* Thumbnail */}
                    <div className="w-12 h-12 bg-gray-200 rounded-md overflow-hidden flex-shrink-0">
                      {/* eslint-disable-next-line @next/next/no-img-element */}
                      <img
                        src={photo.url}
                        alt="Fotoğraf"
                        className="w-full h-full object-cover"
                        onError={(e) => {
                          (e.target as HTMLImageElement).style.display = 'none';
                        }}
                      />
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-xs text-gray-700 truncate">{photo.url}</p>
                      <div className="flex items-center gap-2 mt-0.5">
                        <span className="text-xs text-gray-400">Sıra: {photo.displayOrder}</span>
                        {photo.isMenuPhoto && (
                          <span className="text-xs bg-amber-100 text-amber-700 px-1.5 py-0.5 rounded">
                            Menü
                          </span>
                        )}
                      </div>
                    </div>
                    <button
                      onClick={() => handleDeletePhoto(photo.id)}
                      className="text-red-500 hover:text-red-700 p-1 rounded transition-colors flex-shrink-0"
                      title="Sil"
                    >
                      <svg xmlns="http://www.w3.org/2000/svg" className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                        <path strokeLinecap="round" strokeLinejoin="round" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                      </svg>
                    </button>
                  </div>
                ))}
              </div>
            )}

            {/* Add photo form */}
            <div className="border-t border-gray-100 pt-4">
              <p className="text-sm font-medium text-gray-700 mb-3">Fotoğraf Ekle</p>
              {photoError && (
                <div className="mb-3 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-xs">
                  {photoError}
                </div>
              )}
              <form onSubmit={handleAddPhoto} className="space-y-3">
                <div>
                  <label className="block text-xs font-medium text-gray-600 mb-1">URL</label>
                  <input
                    type="url"
                    required
                    value={photoUrl}
                    onChange={(e) => setPhotoUrl(e.target.value)}
                    placeholder="https://..."
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                  />
                </div>
                <div className="flex items-center gap-4">
                  <div className="flex-1">
                    <label className="block text-xs font-medium text-gray-600 mb-1">Sıra</label>
                    <input
                      type="number"
                      min="0"
                      value={photoOrder}
                      onChange={(e) => setPhotoOrder(e.target.value)}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
                    />
                  </div>
                  <div className="flex items-center gap-2 mt-4">
                    <input
                      type="checkbox"
                      id="isMenuPhoto"
                      checked={photoIsMenu}
                      onChange={(e) => setPhotoIsMenu(e.target.checked)}
                      className="w-4 h-4 text-indigo-600 rounded border-gray-300"
                    />
                    <label htmlFor="isMenuPhoto" className="text-sm text-gray-600">Menü Fotoğrafı</label>
                  </div>
                </div>
                <button
                  type="submit"
                  disabled={photoSubmitting}
                  className="px-4 py-2 bg-indigo-600 hover:bg-indigo-700 disabled:bg-indigo-400 text-white text-sm font-semibold rounded-lg transition-colors"
                >
                  {photoSubmitting ? 'Ekleniyor...' : 'Fotoğraf Ekle'}
                </button>
              </form>
            </div>
          </div>
        </div>
      )}
    </AdminLayout>
  );
}
