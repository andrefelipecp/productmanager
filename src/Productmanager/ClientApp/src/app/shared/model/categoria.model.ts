import { IProduto } from 'app/shared/model/produto.model';

export interface ICategoria {
  id?: number;
  nome?: string;
  produtos?: IProduto[];
}

export class Categoria implements ICategoria {
  constructor(public id?: number, public nome?: string, public produtos?: IProduto[]) {}
}
