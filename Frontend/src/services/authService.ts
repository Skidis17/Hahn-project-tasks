import axios from 'axios';

const API_URL = '/api/auth';

type User = {
  id: string;
  email: string;
  token: string;
};

type LoginData = {
  email: string;
  password: string;
};

type RegisterData = {
  email: string;
  password: string;
  confirmPassword: string;
};

class AuthService {
  async login(loginData: LoginData): Promise<User> {
    const response = await axios.post(`${API_URL}/login`, loginData);
    if (response.data.token) {
      localStorage.setItem('user', JSON.stringify(response.data));
    }
    return response.data;
  }

  async register(registerData: RegisterData): Promise<User> {
    const response = await axios.post(`${API_URL}/register`, registerData);
    return response.data;
  }

  logout(): void {
    localStorage.removeItem('user');
  }

  getCurrentUser(): User | null {
    const userStr = localStorage.getItem('user');
    if (userStr) return JSON.parse(userStr);
    return null;
  }

  getAuthHeader(): { Authorization: string } | {} {
    const user = this.getCurrentUser();
    if (user && user.token) {
      return { Authorization: 'Bearer ' + user.token };
    } else {
      return {};
    }
  }
}

export default new AuthService();
