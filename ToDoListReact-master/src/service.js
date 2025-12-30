import axios from 'axios';

// הגדרת כתובת ה-API של השרת שלך
// ודאי שהפורט (5164) תואם למה שרץ אצלך ב-C#

const apiUrl = "http://localhost:5164";
axios.defaults.baseURL = apiUrl;

// 1. Request Interceptor: הוספת ה-JWT Token מה-localStorage לכל בקשה שיוצאת לשרת
axios.interceptors.request.use(config => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}, error => {
    return Promise.reject(error);
});

// 2. Response Interceptor: טיפול בשגיאות שחוזרות מהשרת
axios.interceptors.response.use(
    response => response,
    error => {
        // אם השרת מחזיר 401, זה אומר שהטוקן לא תקף או חסר
        if (error.response && error.response.status === 401) {
            console.warn("Unauthorized! Redirecting to login...");
            localStorage.removeItem('token'); // מחיקת הטוקן הפגום
            window.location.href = '/login'; // הפניה לדף התחברות
        }
        return Promise.reject(error);
    }
);

export default {
    // פונקציות ניהול המשימות הקיימות (מעודכנות לעבודה עם axios)
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

    // 3. פונקציות ה-Authentication החדשות עבור האתגר
    login: async (username, password) => {
        try {
            const result = await axios.post('/login', { Username: username, Password: password });
            // שמירת הטוקן בזיכרון המקומי של הדפדפן
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