using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace .Domain
{
    [Table("produto")]
    public class Produto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Nome { get; set; }
        public int? Quantidade { get; set; }
        public double? Valor { get; set; }

        public long? CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        // jhipster-needle-entity-add-field - JHipster will add fields here, do not remove

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var produto = obj as Produto;
            if (produto?.Id == null || produto?.Id == 0 || Id == 0) return false;
            return EqualityComparer<long>.Default.Equals(Id, produto.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "Produto{" +
                    $"ID='{Id}'" +
                    $", Nome='{Nome}'" +
                    $", Quantidade='{Quantidade}'" +
                    $", Valor='{Valor}'" +
                    "}";
        }
    }
}
