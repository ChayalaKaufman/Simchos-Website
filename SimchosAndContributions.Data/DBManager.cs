using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchosAndContributors.Data
{
    public class DBManager
    {
        private string _connectionString;

        public DBManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Contributor> GetContributors(string searchText)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            if (searchText != null)
            {
                cmd.CommandText = @"SELECT * FROM Contributors WHERE FirstName Like '%' + @text +'%'
                            OR LastName LIKE '%' + @text +'%'";
                cmd.Parameters.AddWithValue("@text", searchText);
            }
            else
            {
                cmd.CommandText = @"SELECT * FROM Contributors";
            }

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Contributor> contributors = new List<Contributor>();
            while (reader.Read())
            {
                Contributor c = new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    DateCreated = (DateTime)reader["DateCreated"]
                };
                contributors.Add(c);
            }
            conn.Close();
            conn.Dispose();
            GetBalance(contributors);
            return contributors;
        }

        private void GetBalance(IEnumerable<Contributor> contributors)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Amount FROM Deposits WHERE ContributorId = @id";
            conn.Open();

            foreach (Contributor c in contributors)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", c.Id);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    c.Balance += +(decimal)reader["Amount"];
                }
                reader.Close();
            }
            conn.Close();
            conn.Dispose();

            SqlConnection conn2 = new SqlConnection(_connectionString);
            SqlCommand cmd2 = conn2.CreateCommand();
            cmd2.CommandText = @"SELECT Amount FROM ContributionsSimchos
                            WHERE ContributorId = @id2";
            conn2.Open();

            foreach (Contributor c in contributors)
            {
                cmd2.Parameters.Clear();
                cmd2.Parameters.AddWithValue("@id2", c.Id);
                SqlDataReader reader2 = cmd2.ExecuteReader();
                while (reader2.Read())
                {
                    c.Balance += -(decimal)reader2["Amount"];
                }
                reader2.Close();
            }
            conn2.Close();
            conn2.Dispose();
        }

        public void AddContributor(Contributor c, decimal deposit)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Contributors VALUES (@firstName,@lastName,@cell,@date,@alwaysInclude)
                    SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@firstName", c.FirstName);
            cmd.Parameters.AddWithValue("@lastName", c.LastName);
            cmd.Parameters.AddWithValue("@cell", c.CellNumber);
            cmd.Parameters.AddWithValue("@date", c.DateCreated);
            cmd.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
            conn.Open();
            c.Id = (int)(decimal)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
            Deposit d = new Deposit
            {
                Amount = deposit,
                ContributorId = c.Id,
                Date = DateTime.Now
            };
            AddDeposit(d);
        }

        public void AddDeposit(Deposit deposit)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Deposits VALUES (@amt,@id,@date)";
            cmd.Parameters.AddWithValue("@amt", deposit.Amount);
            cmd.Parameters.AddWithValue("@id", deposit.ContributorId);
            cmd.Parameters.AddWithValue("@date", deposit.Date);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public void EditContributor(Contributor c)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Contributors 
                                SET FirstName = @firstName,LastName=@lastName, CellNumber=@cell,
                                DateCreated=@date,AlwaysInclude=@alwaysInclude
                                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@firstName", c.FirstName);
            cmd.Parameters.AddWithValue("@lastName", c.LastName);
            cmd.Parameters.AddWithValue("@cell", c.CellNumber);
            cmd.Parameters.AddWithValue("@date", c.DateCreated);
            cmd.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
            cmd.Parameters.AddWithValue("@id", c.Id);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public IEnumerable<SimchaView> GetSimchos()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * From Simchos";
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Simcha> simchos = new List<Simcha>();
            while (reader.Read())
            {
                simchos.Add(new Simcha
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"]
                });
            }
            conn.Close();
            conn.Dispose();
            IEnumerable<SimchaView> Simchos = GetSimchaViews(simchos);
            return Simchos;
        }

        public int GetTotalContributors()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select count(*) FROM Contributors";
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        private IEnumerable<SimchaView> GetSimchaViews(List<Simcha> simchos)
        {
            List<SimchaView> simchaViews = new List<SimchaView>();

            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"select COUNT(*) As 'Count', ISNULL(SUM(Amount), 0) AS 'Total' from 
                                    ContributionsSimchos where SimchaId = @id";

            conn.Open();
            foreach (Simcha s in simchos)
            {
                SimchaView simcha = new SimchaView
                {
                    Name = s.Name,
                    Date = s.Date,
                    Id = s.Id,
                };
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", simcha.Id);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                {
                    simcha.TotalContributed = (int)reader["Count"];
                    simcha.Total = (decimal)reader["Total"];
                }
                simchaViews.Add(simcha);
                reader.Close();
            }
            conn.Close();
            conn.Dispose();
            return simchaViews;
        }

        public void AddSimcha(Simcha s)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Simchos VALUES (@name,@date)";
            cmd.Parameters.AddWithValue("@name", s.Name);
            cmd.Parameters.AddWithValue("@date", s.Date);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public IEnumerable<Contribution> GetContributions(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Contributors c";
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<Contribution> contributions = new List<Contribution>();
            while (reader.Read())
            {
                Contribution c = new Contribution
                {
                    ContributorId = (int)reader["Id"],
                    Name = (string)reader["FirstName"] + " " + (string)reader["LastName"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"]
                };
                contributions.Add(c);
            }
            conn.Close();
            conn.Dispose();

            SqlConnection conn2 = new SqlConnection(_connectionString);
            SqlCommand cmd2 = conn2.CreateCommand();
            cmd2.CommandText = "SELECT * FROM ContributionsSimchos WHERE SimchaId = @id";
            cmd2.Parameters.AddWithValue("@id", id);
            conn2.Open();
            SqlDataReader reader2 = cmd2.ExecuteReader();
            while (reader2.Read())
            {
                Contribution cont = contributions.Find(con => con.ContributorId == (int)reader2["ContributorId"]);
                cont.Amount = (decimal)reader2["Amount"];
                cont.Contributed = true;
            }
            conn2.Close();
            conn2.Dispose();
            GetBalanceForContributions(contributions);
            return contributions;
        }

        private void GetBalanceForContributions(IEnumerable<Contribution> contributions)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Amount FROM Deposits WHERE ContributorId = @id";
            conn.Open();

            foreach (Contribution c in contributions)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", c.ContributorId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    c.Balance += +(decimal)reader["Amount"];
                }
                reader.Close();
            }
            conn.Close();
            conn.Dispose();

            SqlConnection conn2 = new SqlConnection(_connectionString);
            SqlCommand cmd2 = conn2.CreateCommand();
            cmd2.CommandText = @"SELECT Amount FROM ContributionsSimchos
                            WHERE ContributorId = @id2";
            conn2.Open();

            foreach (Contribution c in contributions)
            {
                cmd2.Parameters.Clear();
                cmd2.Parameters.AddWithValue("@id2", c.ContributorId);
                SqlDataReader reader2 = cmd2.ExecuteReader();
                while (reader2.Read())
                {
                    c.Balance += -(decimal)reader2["Amount"];
                }
                reader2.Close();
            }
            conn2.Close();
            conn2.Dispose();
        }

        public HistoryView GetHistory(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Deposits WHERE ContributorId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            HistoryView historyView = new HistoryView();
            historyView.History = new List<History>();
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                History h = new History
                {
                    Amount = +(decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"],
                    Action = "Deposit"
                };
                historyView.History.Add(h);
            }
            conn.Close();
            conn.Dispose();
            historyView.History.AddRange(GetDonations(id));
            historyView.History.OrderByDescending(h => h.Date);

            foreach (History h in historyView.History)
            {
                historyView.Balance += h.Amount;
            }
            return historyView;
        }

        private List<History> GetDonations(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Amount, cs.Date ,Name FROM ContributionsSimchos cs 
                    JOIN simchos s ON s.Id = SimchaId
                    WHERE ContributorId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            List<History> history = new List<History>();
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                History h = new History
                {
                    Amount = -(decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"],
                    Action = "Contribution to " + (string)reader["Name"],
                };
                history.Add(h);
            }

            return history;
        }

        public void UpdateContributions(List<Contribution> contributions, int simchaId)
        {
            IEnumerable<Contribution> filtered = contributions.Where(c => c.Contributed == true);
            List<Contribution> conts = filtered.ToList();

            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT * FROM ContributionsSimchos where SimchaId = @simchaId";
            cmd.Parameters.AddWithValue("@simchaId", simchaId);
            conn.Open();
            var reader = cmd.ExecuteReader();
            var existingDonations = new List<Donation>();
            while (reader.Read())
            {
                if (reader["ContributorId"] == DBNull.Value)
                {
                    AddDonations(conts, simchaId);
                    return;
                }
                existingDonations.Add(new Donation
                {
                    ContributorId = (int)reader["ContributorId"],
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"]
                });
            }
            conn.Close();
            conn.Dispose();
            List<Contribution> updates = new List<Contribution>();
            List<Contribution> delete = new List<Contribution>();

            foreach (var cont in conts)
            {
                Donation don = existingDonations.Find(d => d.ContributorId == cont.ContributorId);
                if (don != null)
                {
                    if (cont.Amount != don.Amount)
                    {
                        updates.Add(cont);
                    }
                    delete.Add(cont);
                }
            }

            foreach (Contribution c in delete)
            {
                conts.Remove(c);
            }
            if (updates.Count > 0)
            {
                EditDonations(updates, simchaId);
            }
            if (conts.Count > 0)
            {
                AddDonations(conts, simchaId);
            }
        }

        private void AddDonations(List<Contribution> conts, int simchaId)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO ContributionsSimchos VALUES (@contId,@simchaId,@amt,@date)";
            conn.Open();
            foreach (var cont in conts)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@contId", cont.ContributorId);
                cmd.Parameters.AddWithValue("@simchaId", simchaId);
                cmd.Parameters.AddWithValue("@amt", cont.Amount);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
            conn.Close();
            conn.Dispose();
        }

        private void EditDonations(List<Contribution> updates, int simchaId)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE ContributionsSimchos
                SET amount = @amt, date=@date
                WHERE ContributorId = @contId AND SimchaId = @simchaId";
            conn.Open();
            foreach (var cont in updates)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@contId", cont.ContributorId);
                cmd.Parameters.AddWithValue("@simchaId", simchaId);
                cmd.Parameters.AddWithValue("@amt", cont.Amount);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
            conn.Close();
            conn.Dispose();
        }

        public decimal GetTotalOnHand()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ISNULL(sum(amount), 0) AS 'Deposits' from Deposits d";
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            decimal deposits = (decimal)reader["Deposits"];
            conn.Close();
            conn.Dispose();


            SqlConnection conn2 = new SqlConnection(_connectionString);
            SqlCommand cmd2 = conn2.CreateCommand();
            cmd2.CommandText = @"SELECT ISNULL(sum(amount), 0) AS 'Donations' from ContributionsSimchos";
            conn2.Open();
            SqlDataReader reader2 = cmd2.ExecuteReader();
            reader2.Read();
            decimal donations = (decimal)reader2["Donations"];
            conn2.Close();
            conn2.Dispose();

            return deposits - donations;
        }
    }
}
