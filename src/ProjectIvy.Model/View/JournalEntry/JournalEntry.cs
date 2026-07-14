namespace ProjectIvy.Model.View.JournalEntry;

public class JournalEntry
{
    public JournalEntry(Database.Main.User.JournalEntry x)
    {
        Date = x.Date;
        Entry = x.Entry;
        Created = x.Created;
        Modified = x.Modified;
    }

    public DateOnly Date { get; set; }

    public string Entry { get; set; }

    public DateTime Created { get; set; }

    public DateTime Modified { get; set; }
}
