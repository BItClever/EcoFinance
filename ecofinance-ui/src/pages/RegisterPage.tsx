import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { register } from "../api/auth";

const RegisterPage = () => {
  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    try {
      await register(userName, email, password);
      navigate("/login");
    } catch (err: any) {
      setError("Ошибка регистрации");
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h2>Регистрация</h2>
      <input
        type="text"
        placeholder="Имя пользователя"
        value={userName}
        onChange={e => setUserName(e.target.value)}
        required
      />
      <input
        type="email"
        placeholder="Email"
        value={email}
        onChange={e => setEmail(e.target.value)}
        required
      />
      <input
        type="password"
        placeholder="Пароль"
        value={password}
        onChange={e => setPassword(e.target.value)}
        required
      />
      <button type="submit">Зарегистрироваться</button>
      {error && <div style={{ color: "red" }}>{error}</div>}
      <div>
        Уже есть аккаунт? <a href="/login">Войти</a>
      </div>
    </form>
  );
};

export default RegisterPage;