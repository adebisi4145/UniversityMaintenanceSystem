import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { PriorityBadge, StatusBadge } from './StatusBadge';
import { Priority, RequestStatus } from '../types';

describe('StatusBadge', () => {
  it('renders the readable status label', () => {
    render(<StatusBadge status={RequestStatus.InProgress} />);
    expect(screen.getByText('In Progress')).toBeInTheDocument();
  });

  it('renders Completed with the completed style', () => {
    const { container } = render(<StatusBadge status={RequestStatus.Completed} />);
    expect(container.querySelector('.badge-completed')).toBeTruthy();
  });
});

describe('PriorityBadge', () => {
  it('renders the priority label', () => {
    render(<PriorityBadge priority={Priority.High} />);
    expect(screen.getByText('High')).toBeInTheDocument();
  });
});
