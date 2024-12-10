import axios from 'axios';

const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_BACKEND_URL, // URL du backend
  timeout: 5000, 
  headers: {
    'Content-Type': 'application/json', 
  },
});

export default apiClient;
