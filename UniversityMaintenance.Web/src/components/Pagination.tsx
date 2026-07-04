interface Props {
  page: number;
  totalPages: number;
  totalCount: number;
  onPage: (page: number) => void;
}

export function Pagination({ page, totalPages, totalCount, onPage }: Props) {
  if (totalCount === 0) return null;
  return (
    <div className="pagination">
      <button className="btn btn-ghost" disabled={page <= 1} onClick={() => onPage(page - 1)}>
        ‹ Prev
      </button>
      <span className="pagination-info">
        Page {page} of {Math.max(totalPages, 1)} · {totalCount} total
      </span>
      <button
        className="btn btn-ghost"
        disabled={page >= totalPages}
        onClick={() => onPage(page + 1)}
      >
        Next ›
      </button>
    </div>
  );
}
