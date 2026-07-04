import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';
import { Pagination } from './Pagination';

describe('Pagination', () => {
  it('renders nothing when there are no results', () => {
    const { container } = render(
      <Pagination page={1} totalPages={0} totalCount={0} onPage={() => {}} />,
    );
    expect(container.firstChild).toBeNull();
  });

  it('disables Prev on the first page and advances on Next', async () => {
    const onPage = vi.fn();
    render(<Pagination page={1} totalPages={3} totalCount={24} onPage={onPage} />);

    expect(screen.getByRole('button', { name: /prev/i })).toBeDisabled();

    await userEvent.click(screen.getByRole('button', { name: /next/i }));
    expect(onPage).toHaveBeenCalledWith(2);
  });
});
