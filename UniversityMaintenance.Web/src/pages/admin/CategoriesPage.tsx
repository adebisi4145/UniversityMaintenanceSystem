import { useEffect, useState } from 'react';
import { errorMessage } from '../../api/client';
import { categoriesApi } from '../../api/endpoints';
import { useToast } from '../../components/Toast';
import type { CategoryDto } from '../../types';

export function CategoriesPage() {
  const toast = useToast();
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');

  const load = () =>
    categoriesApi.getAll().then(setCategories).catch((err) => toast.error(errorMessage(err)));

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const add = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await categoriesApi.create(name, description || undefined);
      toast.success('Category added');
      setName('');
      setDescription('');
      load();
    } catch (err) {
      toast.error(errorMessage(err));
    }
  };

  const remove = async (id: string) => {
    if (!confirm('Delete this category?')) return;
    try {
      await categoriesApi.remove(id);
      toast.success('Category deleted');
      load();
    } catch (err) {
      toast.error(errorMessage(err));
    }
  };

  return (
    <div className="page">
      <header className="page-head">
        <h1>Categories</h1>
      </header>

      <form onSubmit={add} className="card form">
        <div className="grid-2">
          <label className="field">
            <span>Name</span>
            <input required value={name} onChange={(e) => setName(e.target.value)} />
          </label>
          <label className="field">
            <span>Description</span>
            <input value={description} onChange={(e) => setDescription(e.target.value)} />
          </label>
        </div>
        <div className="form-actions">
          <button className="btn btn-primary" type="submit">
            Add category
          </button>
        </div>
      </form>

      <div className="card">
        <div className="table-wrap">
          <table className="table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Description</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {categories.map((c) => (
                <tr key={c.id}>
                  <td className="strong">{c.name}</td>
                  <td>{c.description ?? '—'}</td>
                  <td>
                    <button className="link danger" onClick={() => remove(c.id)}>
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
