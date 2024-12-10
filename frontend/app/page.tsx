import Navbar from '@components/navBar/Navbar';
import TestBackend from '@components/TestBackend';

export default function HomePage(): JSX.Element {
  return (
    <div>
      <Navbar />
      <h2>Bienvenue sur la page d'accueil</h2>
      <TestBackend />
    </div>
  );
}
