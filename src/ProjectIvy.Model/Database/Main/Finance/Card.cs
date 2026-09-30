using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(Card), Schema = nameof(Finance))]
public class Card : UserEntity, IHasValueId, IHasName
{
    public Bank Bank { get; set; }

    public int BankId { get; set; }

    public CardType CardType { get; set; }

    public int CardTypeId { get; set; }

    public DateTime Expires { get; set; }

    [Key]
    public int Id { get; set; }

    public bool IsActive { get; set; }

    public DateTime Issued { get; set; }

    public string LastFourDigits { get; set; }

    public string Name { get; set; }

    public string ValueId { get; set; }
}
