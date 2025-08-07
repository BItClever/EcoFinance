import { removeToken } from "../utils/auth";
import { useNavigate } from "react-router-dom";

const Header = () => {
  const navigate = useNavigate();
  const handleLogout = () => {
    removeToken();
    navigate("/login");
  };
  return (
    <header>
      <button onClick={handleLogout}>Выйти</button>
    </header>
  );
};

export default Header;