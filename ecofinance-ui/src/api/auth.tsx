import axios from "axios";

const API_URL = process.env.REACT_APP_API_URL || "http://localhost:5000/api";

export async function login(userName: string, password: string): Promise<string> {
  const res = await axios.post(`${API_URL}/auth/login`, { userName, password });
  return res.data.token; // ожидаем { token: "..." }
}

export async function register(userName: string, email: string, password: string): Promise<void> {
  await axios.post(`${API_URL}/auth/register`, { userName, email, password });
}