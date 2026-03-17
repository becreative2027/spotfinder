'use client';

import { useEffect, useState } from 'react';
import AdminLayout from '@/components/AdminLayout';
import LoadingSpinner from '@/components/LoadingSpinner';
import { getVenues, getPendingReviews, getUsers } from '@/lib/api';

interface Stats {
  totalVenues: number;
  pendingReviews: number;
  totalUsers: number;
}

export default function DashboardPage() {
  const [stats, setStats] = useState<Stats | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    async function fetchStats() {
      try {
        const [venuesRes, reviewsRes, usersRes] = await Promise.all([
          getVenues(1, 1),
          getPendingReviews(),
          getUsers(1, 1),
        ]);
        setStats({
          totalVenues: venuesRes.totalCount,
          pendingReviews: reviewsRes.length,
          totalUsers: usersRes.totalCount,
        });
      } catch (err: unknown) {
        const message = err instanceof Error ? err.message : 'Veriler alınamadı.';
        setError(message);
      } finally {
        setLoading(false);
      }
    }
    fetchStats();
  }, []);

  const statCards = [
    {
      label: 'Toplam Mekân',
      value: stats?.totalVenues ?? 0,
      color: 'bg-indigo-50 border-indigo-200',
      iconBg: 'bg-indigo-100',
      textColor: 'text-indigo-700',
      icon: (
        <svg xmlns="http://www.w3.org/2000/svg" className="w-6 h-6 text-indigo-600" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
          <path strokeLinecap="round" strokeLinejoin="round" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
          <path strokeLinecap="round" strokeLinejoin="round" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
        </svg>
      ),
    },
    {
      label: 'Bekleyen Yorum',
      value: stats?.pendingReviews ?? 0,
      color: 'bg-amber-50 border-amber-200',
      iconBg: 'bg-amber-100',
      textColor: 'text-amber-700',
      icon: (
        <svg xmlns="http://www.w3.org/2000/svg" className="w-6 h-6 text-amber-600" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
          <path strokeLinecap="round" strokeLinejoin="round" d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z" />
        </svg>
      ),
    },
    {
      label: 'Toplam Kullanıcı',
      value: stats?.totalUsers ?? 0,
      color: 'bg-green-50 border-green-200',
      iconBg: 'bg-green-100',
      textColor: 'text-green-700',
      icon: (
        <svg xmlns="http://www.w3.org/2000/svg" className="w-6 h-6 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
          <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
        </svg>
      ),
    },
  ];

  return (
    <AdminLayout title="Dashboard">
      {loading && <LoadingSpinner />}

      {error && (
        <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
          {error}
        </div>
      )}

      {!loading && !error && stats && (
        <>
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-6 mb-8">
            {statCards.map((card) => (
              <div
                key={card.label}
                className={`bg-white rounded-xl border p-6 flex items-center gap-4 shadow-sm ${card.color}`}
              >
                <div className={`w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0 ${card.iconBg}`}>
                  {card.icon}
                </div>
                <div>
                  <p className="text-sm text-gray-500 font-medium">{card.label}</p>
                  <p className={`text-3xl font-bold mt-0.5 ${card.textColor}`}>
                    {card.value.toLocaleString('tr-TR')}
                  </p>
                </div>
              </div>
            ))}
          </div>

          <div className="bg-white rounded-xl border border-gray-200 p-6 shadow-sm">
            <h2 className="text-lg font-semibold text-gray-800 mb-2">Hızlı Erişim</h2>
            <p className="text-sm text-gray-500">
              SpotFinder yönetim paneline hoş geldiniz. Sol menüden mekânları, etiketleri, yorumları ve kullanıcıları yönetebilirsiniz.
            </p>
          </div>
        </>
      )}
    </AdminLayout>
  );
}
