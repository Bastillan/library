import axios from 'axios';

const api = axios.create({
    baseURL: '/api',
    headers: {
        'Content-Type': 'application/json',
    },
});

api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

api.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;

        if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;

            try {
                const refreshToken = localStorage.getItem('refreshToken');
                const response = await axios.post('/api/Users/refresh-token', {
                    accessToken: localStorage.getItem('token'),
                    refreshToken: refreshToken,
                });

                const { authToken, refreshToken: newRefreshToken } = response.data;

                localStorage.setItem('token', authToken);
                localStorage.setItem('refreshToken', newRefreshToken);

                originalRequest.headers.Authorization = `Bearer ${authToken}`;
                return api(originalRequest);
            } catch (error) {
                localStorage.removeItem('token');
                localStorage.removeItem('refreshToken');
                window.location.href = '/account/login';
                return Promise.reject(error);
            }
        }
        return Promise.reject(error);
    }
);

export default api;