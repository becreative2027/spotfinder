import { getToken, removeToken } from './auth';

const AUTH_API = process.env.NEXT_PUBLIC_AUTH_API || 'http://localhost:5001';
const VENUE_API = process.env.NEXT_PUBLIC_VENUE_API || 'http://localhost:5002';
const REVIEW_API = process.env.NEXT_PUBLIC_REVIEW_API || 'http://localhost:5004';

// ─── Types ────────────────────────────────────────────────────────────────────

export interface User {
  id: string;
  email: string;
  fullName: string;
  avatarUrl?: string;
  provider?: string;
  role: string;
  isActive?: boolean;
  createdAt?: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiry: string;
  user: User;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface Photo {
  id: string;
  url: string;
  isMenuPhoto: boolean;
  displayOrder: number;
}

export interface ConceptTag {
  id: string;
  nameTr: string;
  nameEn: string;
  isSystem: boolean;
  isActive: boolean;
}

export interface District {
  id: string;
  name: string;
  city: string;
}

export interface Venue {
  id: string;
  name: string;
  description?: string;
  districtId: string;
  districtName?: string;
  address: string;
  parkingStatus: string;
  lat: number;
  lng: number;
  averageRating?: number;
  reviewCount?: number;
  isActive: boolean;
  createdAt: string;
  shareUrl?: string;
  photos: Photo[];
  conceptTags: ConceptTag[];
}

export interface VenuePayload {
  name: string;
  description?: string;
  districtId: string;
  address: string;
  parkingStatus: string;
  lat: number;
  lng: number;
  conceptTagIds?: string[];
}

export interface Review {
  id: string;
  venueId: string;
  userId: string;
  body: string;
  rating: number;
  status: string;
  createdAt: string;
}

// ─── Core fetch helper ────────────────────────────────────────────────────────

export async function authFetch(url: string, options: RequestInit = {}): Promise<Response> {
  const token = getToken();
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(options.headers as Record<string, string> || {}),
  };
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const response = await fetch(url, { ...options, headers });

  if (response.status === 401) {
    removeToken();
    if (typeof window !== 'undefined') {
      window.location.href = '/login';
    }
  }

  return response;
}

// ─── Auth ─────────────────────────────────────────────────────────────────────

export async function login(email: string, password: string): Promise<LoginResponse> {
  const res = await fetch(`${AUTH_API}/api/v1/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password }),
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.message || 'Giriş başarısız.');
  }
  return res.json();
}

export async function getMe(): Promise<User> {
  const res = await authFetch(`${AUTH_API}/api/v1/auth/me`);
  if (!res.ok) throw new Error('Kullanıcı bilgisi alınamadı.');
  return res.json();
}

export async function getUsers(page = 1, pageSize = 20): Promise<PaginatedResponse<User>> {
  const res = await authFetch(`${AUTH_API}/api/v1/admin/users?page=${page}&pageSize=${pageSize}`);
  if (!res.ok) throw new Error('Kullanıcılar alınamadı.');
  return res.json();
}

// ─── Venues ───────────────────────────────────────────────────────────────────

export async function getVenues(page = 1, pageSize = 20): Promise<PaginatedResponse<Venue>> {
  const res = await authFetch(`${VENUE_API}/api/v1/venues?page=${page}&pageSize=${pageSize}`);
  if (!res.ok) throw new Error('Mekânlar alınamadı.');
  return res.json();
}

export async function getVenue(id: string): Promise<Venue> {
  const res = await authFetch(`${VENUE_API}/api/v1/venues/${id}`);
  if (!res.ok) throw new Error('Mekân alınamadı.');
  return res.json();
}

export async function createVenue(payload: VenuePayload): Promise<Venue> {
  const res = await authFetch(`${VENUE_API}/api/v1/venues`, {
    method: 'POST',
    body: JSON.stringify(payload),
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.message || 'Mekân oluşturulamadı.');
  }
  return res.json();
}

export async function updateVenue(id: string, payload: VenuePayload): Promise<Venue> {
  const res = await authFetch(`${VENUE_API}/api/v1/venues/${id}`, {
    method: 'PUT',
    body: JSON.stringify(payload),
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.message || 'Mekân güncellenemedi.');
  }
  return res.json();
}

export async function deleteVenue(id: string): Promise<void> {
  const res = await authFetch(`${VENUE_API}/api/v1/venues/${id}`, { method: 'DELETE' });
  if (!res.ok) throw new Error('Mekân silinemedi.');
}

// ─── Districts ────────────────────────────────────────────────────────────────

export async function getDistricts(): Promise<District[]> {
  const res = await authFetch(`${VENUE_API}/api/v1/districts`);
  if (!res.ok) throw new Error('İlçeler alınamadı.');
  return res.json();
}

// ─── Concept Tags ─────────────────────────────────────────────────────────────

export async function getConceptTags(): Promise<ConceptTag[]> {
  const res = await authFetch(`${VENUE_API}/api/v1/concept-tags`);
  if (!res.ok) throw new Error('Etiketler alınamadı.');
  return res.json();
}

export async function createConceptTag(nameTr: string, nameEn: string): Promise<ConceptTag> {
  const res = await authFetch(`${VENUE_API}/api/v1/concept-tags`, {
    method: 'POST',
    body: JSON.stringify({ nameTr, nameEn }),
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.message || 'Etiket oluşturulamadı.');
  }
  return res.json();
}

export async function deleteConceptTag(id: string): Promise<void> {
  const res = await authFetch(`${VENUE_API}/api/v1/concept-tags/${id}`, { method: 'DELETE' });
  if (!res.ok) throw new Error('Etiket silinemedi.');
}

// ─── Photos ───────────────────────────────────────────────────────────────────

export async function addPhoto(venueId: string, url: string, isMenuPhoto: boolean, displayOrder: number): Promise<Photo> {
  const res = await authFetch(`${VENUE_API}/api/v1/venues/${venueId}/photos`, {
    method: 'POST',
    body: JSON.stringify({ url, isMenuPhoto, displayOrder }),
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.message || 'Fotoğraf eklenemedi.');
  }
  return res.json();
}

export async function deletePhoto(venueId: string, photoId: string): Promise<void> {
  const res = await authFetch(`${VENUE_API}/api/v1/venues/${venueId}/photos/${photoId}`, { method: 'DELETE' });
  if (!res.ok) throw new Error('Fotoğraf silinemedi.');
}

// ─── Reviews ──────────────────────────────────────────────────────────────────

export async function getPendingReviews(): Promise<Review[]> {
  const res = await authFetch(`${REVIEW_API}/api/v1/reviews/pending`);
  if (!res.ok) throw new Error('Bekleyen yorumlar alınamadı.');
  return res.json();
}

export async function getVenueReviews(venueId: string): Promise<Review[]> {
  const res = await authFetch(`${REVIEW_API}/api/v1/reviews/venue/${venueId}`);
  if (!res.ok) throw new Error('Yorumlar alınamadı.');
  return res.json();
}

export async function approveReview(id: string): Promise<void> {
  const res = await authFetch(`${REVIEW_API}/api/v1/reviews/${id}/approve`, { method: 'PUT' });
  if (!res.ok) throw new Error('Yorum onaylanamadı.');
}

export async function rejectReview(id: string): Promise<void> {
  const res = await authFetch(`${REVIEW_API}/api/v1/reviews/${id}/reject`, { method: 'PUT' });
  if (!res.ok) throw new Error('Yorum reddedilemedi.');
}

export async function deleteReview(id: string): Promise<void> {
  const res = await authFetch(`${REVIEW_API}/api/v1/reviews/${id}`, { method: 'DELETE' });
  if (!res.ok) throw new Error('Yorum silinemedi.');
}
