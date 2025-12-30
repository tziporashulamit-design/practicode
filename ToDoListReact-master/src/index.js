import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import App from './App';
import Login from './Login';

// פונקציה שבודקת אם קיים טוקן בזיכרון של הדפדפן
const isAuthenticated = () => !!localStorage.getItem('token');

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <Router>
    <Routes>
      {/* דף ההתחברות והרשמה */}
      <Route path="/login" element={<Login />} />

      {/* דף המשימות הראשי - הגנה: אם לא מחובר, שלח ללוגין */}
      <Route 
        path="/" 
        element={isAuthenticated() ? <App /> : <Navigate to="/login" />} 
      />

      {/* ניתוב ברירת מחדל לכל כתובת אחרת */}
      <Route path="*" element={<Navigate to="/" />} />
    </Routes>
  </Router>
);