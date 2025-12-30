import axios from 'axios';

const apiUrl = "http://localhost:5164";
axios.defaults.baseURL = process.env.REACT_APP_API_URL;
axios.interceptors.request.use(config => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}, error => {
    return Promise.reject(error);
});

axios.interceptors.response.use(
    response => response,
    error => {
        if (error.response && error.response.status === 401) {
            console.warn("Unauthorized! Redirecting to login...");
            localStorage.removeItem('token'); 
            window.location.href = '/login'; 
        }
        return Promise.reject(error);
    }
);

export default {
    getTasks: async () => {
        const result = await axios.get('/items');    
        return result.data;
    },

    addTask: async (name) => {
        const result = await axios.post('/items', { name, isComplete: false });
        return result.data;
    },

    setCompleted: async (id, isComplete) => {
        const result = await axios.put(`/items/${id}`, { isComplete });
        return result.data;
    },

    deleteTask: async (id) => {
        const result = await axios.delete(`/items/${id}`);
        return result.data;
    },

    login: async (username, password) => {
        try {
            const result = await axios.post('/login', { Username: username, Password: password });
            if (result.data && result.data.token) {
                localStorage.setItem('token', result.data.token);
            }
            return result.data;
        } catch (error) {
            console.error("Login failed", error);
            throw error;
        }
    },

    register: async (username, password) => {
        try {
            const result = await axios.post('/register', { Username: username, Password: password });
            return result.data;
        } catch (error) {
            console.error("Registration failed", error);
            throw error;
        }
    },

    logout: () => {
        localStorage.removeItem('token');
        window.location.href = '/login';
    }
};