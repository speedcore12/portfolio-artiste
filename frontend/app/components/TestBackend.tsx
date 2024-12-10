'use client';

import { useEffect, useState } from 'react';
import apiClient from '../utils/apiClient';

export default function TestBackend(): JSX.Element {
  const [data, setData] = useState<any>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        // Appel à l'endpoint /test-db
        const response = await apiClient.get('/test-db');
        setData(response.data);
      } catch (err) {
        console.error('Erreur lors de la connexion au backend :', err);
        setError('Impossible de récupérer les données du backend.');
      }
    };

    fetchData();
  }, []);

  if (error) {
    return <div>Erreur : {error}</div>;
  }

  return (
    <div>
      <h1>Données du backend :</h1>
      <pre>{data ? JSON.stringify(data, null, 2) : 'Chargement...'}</pre>
    </div>
  );
}
