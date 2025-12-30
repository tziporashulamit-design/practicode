import React, { useState } from 'react';
import service from './service';
import { useNavigate } from 'react-router-dom';

function Login() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [isRegister, setIsRegister] = useState(false);
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (isRegister) {
                await service.register(username, password);
                alert("נרשמת בהצלחה! עכשיו אפשר להתחבר");
                setIsRegister(false);
           } else {
    await service.login(username, password);
    // במקום navigate, נשתמש ברענון דף מלא כדי שה-index.js יקרא מחדש את הטוקן
    window.location.href = "/"; 
}
        } catch (error) {
            alert("ההתחברות נכשלה. בדקי שם משתמש וסיסמה");
        }
    };

    const containerStyle = {
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        background: 'linear-gradient(135deg, #f7f3ef, #ece6df)',
        fontFamily: '"Inter", "Segoe UI", system-ui, sans-serif',
    };

    const cardStyle = {
        background: '#fffaf6',
        borderRadius: '22px',
        boxShadow: '0 18px 40px rgba(0, 0, 0, 0.1), 0 2px 6px rgba(0, 0, 0, 0.05)',
        padding: '50px 45px',
        width: '100%',
        maxWidth: '420px',
        textAlign: 'center',
    };

    const titleStyle = {
        margin: '0 0 30px 0',
        fontSize: '32px',
        fontWeight: '700',
        color: '#3b2f2f',
        letterSpacing: '-0.5px',
    };

    const inputContainerStyle = {
        marginBottom: '20px',
        textAlign: 'right',
    };

    const labelStyle = {
        display: 'block',
        marginBottom: '8px',
        color: '#5c4636',
        fontSize: '14px',
        fontWeight: '500',
    };

    const inputStyle = {
        width: '100%',
        padding: '14px 16px',
        fontSize: '16px',
        borderRadius: '14px',
        border: '1px solid #d9c7b5',
        outline: 'none',
        background: '#fffdfb',
        transition: 'border-color 0.2s, box-shadow 0.2s',
        boxSizing: 'border-box',
    };

    const buttonStyle = {
        width: '100%',
        padding: '14px',
        marginTop: '10px',
        backgroundColor: '#c08457',
        color: 'white',
        border: 'none',
        borderRadius: '14px',
        fontSize: '16px',
        fontWeight: '600',
        cursor: 'pointer',
        transition: 'background 0.2s, transform 0.2s',
    };

    const switchTextStyle = {
        marginTop: '25px',
        color: '#a18b78',
        fontSize: '14px',
    };

    const switchLinkStyle = {
        color: '#c08457',
        cursor: 'pointer',
        fontWeight: '600',
        marginRight: '5px',
    };

    return (
        <div style={containerStyle}>
            <div style={cardStyle}>
                <h1 style={titleStyle}>{isRegister ? "הרשמה" : "התחברות"}</h1>
                <form onSubmit={handleSubmit}>
                    <div style={inputContainerStyle}>
                        <label style={labelStyle}>שם משתמש</label>
                        <input 
                            style={inputStyle} 
                            value={username} 
                            onChange={e => setUsername(e.target.value)} 
                            placeholder="הכנס שם משתמש"
                            required 
                        />
                    </div>
                    <div style={inputContainerStyle}>
                        <label style={labelStyle}>סיסמה</label>
                        <input 
                            type="password" 
                            style={inputStyle} 
                            value={password} 
                            onChange={e => setPassword(e.target.value)} 
                            placeholder="הכנס סיסמה"
                            required 
                        />
                    </div>
                    <button 
                        type="submit" 
                        style={buttonStyle}
                        onMouseOver={e => e.target.style.backgroundColor = '#a66f48'}
                        onMouseOut={e => e.target.style.backgroundColor = '#c08457'}
                    >
                        {isRegister ? "הירשם" : "התחבר"}
                    </button>
                </form>
                <p style={switchTextStyle}>
                    {isRegister ? "כבר יש לך חשבון?" : "משתמש חדש?"}{' '}
                    <span style={switchLinkStyle} onClick={() => setIsRegister(!isRegister)}>
                        {isRegister ? "להתחברות" : "להרשמה"}
                    </span>
                </p>
            </div>
        </div>
    );
}

export default Login;