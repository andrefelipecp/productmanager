using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace .Domain
{
    [Table("categoria")]
    public class Categoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Nome { get; set; }
        public IList<Produto> Produtos { get; set; } = new List<Produto>();

        // jhipster-needle-entity-add-field - JHipster will add fields here, do not remove

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var categoria = obj as Categoria;
            if (categoria?.Id == null || categoria?.Id == 0 || Id == 0) return false;
            return EqualityComparer<long>.Default.Equals(Id, categoria.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "Categoria{" +
                    $"ID='{Id}'" +
                    $", Nome='{Nome}'" +
                    "}";
        }
    }
}
