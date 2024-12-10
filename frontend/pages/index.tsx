import Navbar from '@components/Navbar';
import TestBackend from '@components/TestBackend';

export default function HomePage(): JSX.Element {
  return (
    <div>
      <Navbar />
      <h1>Bienvenue sur la page d'accueil</h1>
      <TestBackend />
    </div>
  );
}
