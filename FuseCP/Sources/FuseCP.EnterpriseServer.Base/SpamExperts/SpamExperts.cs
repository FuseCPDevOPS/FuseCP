// Copyright (C) 2025 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuseCP.EnterpriseServer
{
    public class SpamExperts
    {
        private bool seEnabled;
        private string schemaValue;
        private string urlValue;
        private string userValue;
        private string passwordValue;
        private string errorMailSubject;
        private string errorMailBody;


        public bool SEEnabled
        {
            get { return seEnabled; }
            set { seEnabled = value; }
        }

        public string schema
        {
            get { return schemaValue; }
            set { schemaValue = value; }
        }

        public string url
        {
            get { return urlValue; }
            set { urlValue = value; }
        }

        public string user
        {
            get { return userValue; }
            set { userValue = value; }
        }

        public string password
        {
            get { return passwordValue; }
            set { passwordValue = value; }
        }

        public string ErrorMailSubject
        {
            get { return errorMailSubject; }
            set { errorMailSubject = value; }
        }

        public string ErrorMailBody
        {
            get { return errorMailBody; }
            set { errorMailBody = value; }
        }

    }
}
