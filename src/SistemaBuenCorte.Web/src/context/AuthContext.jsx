import { createContext, useContext, useState, useCallback } from "react";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [sesion, setSesion] = useState(() => {
    try {
      const guardada = sessionStorage.getItem("buen_corte_sesion");
      return guardada ? JSON.parse(guardada) : null;
    } catch {
      return null;
    }
  });

  const login = useCallback((datos) => {
    sessionStorage.setItem("buen_corte_sesion", JSON.stringify(datos));
    setSesion(datos);
  }, []);

  const logout = useCallback(() => {
    sessionStorage.removeItem("buen_corte_sesion");
    setSesion(null);
  }, []);

  return (
    <AuthContext.Provider value={{ sesion, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
