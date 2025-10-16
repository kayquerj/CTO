import { render, screen } from '@testing-library/react';
import HomePage from '../app/page';

describe('HomePage', () => {
  it('renders the hero content', () => {
    render(<HomePage />);
    expect(screen.getByRole('heading', { name: /phase 0 scaffold/i })).toBeInTheDocument();
    expect(screen.getByText(/postgres/i)).toBeInTheDocument();
  });
});
