export interface IProduto {
  id?: number;
  nome?: string;
  quantidade?: number;
  valor?: number;
  categoriaId?: number;
}

export class Produto implements IProduto {
  constructor(public id?: number, public nome?: string, public quantidade?: number, public valor?: number, public categoriaId?: number) {}
}
