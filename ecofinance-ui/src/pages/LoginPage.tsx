import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { login } from "../api/auth";
import { setToken } from "../utils/auth";

const LoginPage = () => {
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    try {
      const token = await login(userName, password);
      setToken(token);
      navigate("/expenses");
    } catch (err: any) {
      setError("Неверный логин или пароль");
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h2>Вход</h2>
      <input
        type="text"
        placeholder="Имя пользователя"
        value={userName}
        onChange={e => setUserName(e.target.value)}
        required
      />
      <input
        type="password"
        placeholder="Пароль"
        value={password}
        onChange={e => setPassword(e.target.value)}
        required
      />
      <button type="submit">Войти</button>
      {error && <div style={{ color: "red" }}>{error}</div>}
      <div>
        Нет аккаунта? <a href="/register">Зарегистрироваться</a>
      </div>
    </form>
  );
};

export default LoginPage;