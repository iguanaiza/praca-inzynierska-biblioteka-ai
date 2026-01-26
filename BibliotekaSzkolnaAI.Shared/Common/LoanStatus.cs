using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.Common
{
    public enum LoanStatus
    {
        PendingApproval, // 1. Utworzone przez finalizację, czeka na akceptację bibliotekarza
        Active,          // 2. Aktywne, wypożyczone
        Overdue,         // 3. Aktywne, ale po terminie
        PendingReturn,   // 4. Użytkownik kliknął "Zwróć", czeka na akceptację zwrotu
        Returned         // 5. Zakończone i zarchiwizowane
    }
}
