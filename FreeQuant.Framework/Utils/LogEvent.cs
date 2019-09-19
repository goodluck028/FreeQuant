using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public static class LogEvent {
        public class EnginLog {
            string content;

            public EnginLog(string content) {
                this.content = content;
            }
            public string Content {
                get {
                    return content;
                }
            }
        }

        public class UserLog {
            string content;

            public UserLog(string content) {
                this.content = content;
            }
            public string Content {
                get {
                    return content;
                }
            }
        }

        public class ErrorLog {
            string error;

            public ErrorLog(string error) {
                this.error = error;
            }
            public string Error {
                get {
                    return error;
                }
            }
        }
    }
}
