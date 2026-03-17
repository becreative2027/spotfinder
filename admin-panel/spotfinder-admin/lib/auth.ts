export const TOKEN_KEY = 'sf_admin_token';

export const getToken = (): string | null =>
  typeof window !== 'undefined' ? localStorage.getItem(TOKEN_KEY) : null;

export const setToken = (t: string): void => {
  localStorage.setItem(TOKEN_KEY, t);
  // Also set cookie for middleware
  document.cookie = `${TOKEN_KEY}=${t}; path=/; max-age=${60 * 60 * 24 * 7}; SameSite=Lax`;
};

export const removeToken = (): void => {
  localStorage.removeItem(TOKEN_KEY);
  // Remove cookie
  document.cookie = `${TOKEN_KEY}=; path=/; max-age=0; SameSite=Lax`;
};

export const isAuthenticated = (): boolean => !!getToken();
