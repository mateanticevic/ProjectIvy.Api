using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(Card), Schema = nameof(Finance))]
public class Card : UserEntity, IHasValueId, IHasName
{
    [Key]
    public int Id { get; set; }

    public string ValueId { get; set; }

    public string Name { get; set; }

    public DateTime Expires { get; set; }

    public DateTime Issued { get; set; }

    public string LastFourDigits { get; set; }

    public bool IsActive { get; set; }

    public int BankId { get; set; }

    public int CardTypeId { get; set; }

    public Bank Bank { get; set; }

    public CardType CardType { get; set; }
}
