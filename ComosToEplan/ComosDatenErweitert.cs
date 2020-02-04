using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ComosToEplan
{

    public partial class MSR
    {

        // Beginn eigner Code
        /// <summary>
        /// Vergleicher auf Wertebene
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareTo(MSR y)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream memoryStreamX = new MemoryStream())
            {
                using (MemoryStream memoryStreamY = new MemoryStream())
                {
                    formatter.Serialize(memoryStreamX, this);
                    formatter.Serialize(memoryStreamY, y);

                    var binaryArrayX = memoryStreamX.GetBuffer();
                    var binaryArrayY = memoryStreamY.GetBuffer();

                    if (binaryArrayX.Length != binaryArrayY.Length)
                        return binaryArrayX.Length.CompareTo(binaryArrayY.Length);

                    for (int i = 0; i < binaryArrayX.Length; i++)
                        if (binaryArrayX[i] != binaryArrayY[i])
                            return binaryArrayX[i].CompareTo(binaryArrayY[i]);
                }
            }

            return 0;
        }
        /// <summary>
        /// eigener Vergleicher überschrieben Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MSR other)
        {
            return (this.CompareTo(other) == 0);
        }
        /// <summary>
        /// Vergleicher überschreibt Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (this.CompareTo((MSR)obj) == 0);
        }
        /// <summary>
        /// eigenr Hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// eigener Vergleicher auf Gleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(MSR x, MSR y)
        {
            return x.Equals(y);
        }
        /// <summary>
        /// eigener Vergleicher auf Ungleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(MSR x, MSR y)
        {
            return !x.Equals(y);
        }
        // Ende eigner Code
    }

    public partial class MSRMSR_Gerät
    {
        // Beginn eigner Code
        /// <summary>
        /// Vergleicher auf Wertebene
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareTo(MSRMSR_Gerät y)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream memoryStreamX = new MemoryStream())
            {
                using (MemoryStream memoryStreamY = new MemoryStream())
                {
                    formatter.Serialize(memoryStreamX, this);
                    formatter.Serialize(memoryStreamY, y);

                    var binaryArrayX = memoryStreamX.GetBuffer();
                    var binaryArrayY = memoryStreamY.GetBuffer();

                    if (binaryArrayX.Length != binaryArrayY.Length)
                        return binaryArrayX.Length.CompareTo(binaryArrayY.Length);

                    for (int i = 0; i < binaryArrayX.Length; i++)
                        if (binaryArrayX[i] != binaryArrayY[i])
                            return binaryArrayX[i].CompareTo(binaryArrayY[i]);
                }
            }

            return 0;
        }
        /// <summary>
        /// eigener Vergleicher überschrieben Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MSRMSR_Gerät other)
        {
            return (this.CompareTo(other) == 0);
        }
        /// <summary>
        /// Vergleicher überschreibt Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (this.CompareTo((MSRMSR_Gerät)obj) == 0);
        }
        /// <summary>
        /// eigenr Hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// eigener Vergleicher auf Gleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(MSRMSR_Gerät x, MSRMSR_Gerät y)
        {
            return x.Equals(y);
        }
        /// <summary>
        /// eigener Vergleicher auf Ungleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(MSRMSR_Gerät x, MSRMSR_Gerät y)
        {
            return !x.Equals(y);
        }
        // Ende eigner Code
    }
    public partial class MSRMSR_GerätEPLAN_DATA
    {
        // Beginn eigner Code
        /// <summary>
        /// Vergleicher auf Wertebene
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareTo(MSRMSR_GerätEPLAN_DATA y)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream memoryStreamX = new MemoryStream())
            {
                using (MemoryStream memoryStreamY = new MemoryStream())
                {
                    formatter.Serialize(memoryStreamX, this);
                    formatter.Serialize(memoryStreamY, y);

                    var binaryArrayX = memoryStreamX.GetBuffer();
                    var binaryArrayY = memoryStreamY.GetBuffer();

                    if (binaryArrayX.Length != binaryArrayY.Length)
                        return binaryArrayX.Length.CompareTo(binaryArrayY.Length);

                    for (int i = 0; i < binaryArrayX.Length; i++)
                        if (binaryArrayX[i] != binaryArrayY[i])
                            return binaryArrayX[i].CompareTo(binaryArrayY[i]);
                }
            }

            return 0;
        }
        /// <summary>
        /// eigener Vergleicher überschrieben Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MSRMSR_GerätEPLAN_DATA other)
        {
            return (this.CompareTo(other) == 0);
        }
        /// <summary>
        /// Vergleicher überschreibt Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (this.CompareTo((MSRMSR_GerätEPLAN_DATA)obj) == 0);
        }
        /// <summary>
        /// eigenr Hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// eigener Vergleicher auf Gleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(MSRMSR_GerätEPLAN_DATA x, MSRMSR_GerätEPLAN_DATA y)
        {
            return x.Equals(y);
        }
        /// <summary>
        /// eigener Vergleicher auf Ungleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(MSRMSR_GerätEPLAN_DATA x, MSRMSR_GerätEPLAN_DATA y)
        {
            return !x.Equals(y);
        }
        // Ende eigner Code
    }

    public partial class MSRMSR_GerätSignal
    {
        // Beginn eigner Code
        /// <summary>
        /// Vergleicher auf Wertebene
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareTo(MSRMSR_GerätSignal y)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream memoryStreamX = new MemoryStream())
            {
                using (MemoryStream memoryStreamY = new MemoryStream())
                {
                    formatter.Serialize(memoryStreamX, this);
                    formatter.Serialize(memoryStreamY, y);

                    var binaryArrayX = memoryStreamX.GetBuffer();
                    var binaryArrayY = memoryStreamY.GetBuffer();

                    if (binaryArrayX.Length != binaryArrayY.Length)
                        return binaryArrayX.Length.CompareTo(binaryArrayY.Length);

                    for (int i = 0; i < binaryArrayX.Length; i++)
                        if (binaryArrayX[i] != binaryArrayY[i])
                            return binaryArrayX[i].CompareTo(binaryArrayY[i]);
                }
            }

            return 0;
        }
        /// <summary>
        /// eigener Vergleicher überschrieben Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MSRMSR_GerätSignal other)
        {
            return (this.CompareTo(other) == 0);
        }
        /// <summary>
        /// Vergleicher überschreibt Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (this.CompareTo((MSRMSR_GerätSignal)obj) == 0);
        }
        /// <summary>
        /// eigenr Hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// eigener Vergleicher auf Gleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(MSRMSR_GerätSignal x, MSRMSR_GerätSignal y)
        {
            return x.Equals(y);
        }
        /// <summary>
        /// eigener Vergleicher auf Ungleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(MSRMSR_GerätSignal x, MSRMSR_GerätSignal y)
        {
            return !x.Equals(y);
        }
        // Ende eigner Code
    }

    public partial class MSRMSR_GerätSignalSignalKonfig
    {
        // Beginn eigner Code
        /// <summary>
        /// Vergleicher auf Wertebene
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareTo(MSRMSR_GerätSignalSignalKonfig y)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream memoryStreamX = new MemoryStream())
            {
                using (MemoryStream memoryStreamY = new MemoryStream())
                {
                    formatter.Serialize(memoryStreamX, this);
                    formatter.Serialize(memoryStreamY, y);

                    var binaryArrayX = memoryStreamX.GetBuffer();
                    var binaryArrayY = memoryStreamY.GetBuffer();

                    if (binaryArrayX.Length != binaryArrayY.Length)
                        return binaryArrayX.Length.CompareTo(binaryArrayY.Length);

                    for (int i = 0; i < binaryArrayX.Length; i++)
                        if (binaryArrayX[i] != binaryArrayY[i])
                            return binaryArrayX[i].CompareTo(binaryArrayY[i]);
                }
            }

            return 0;
        }
        /// <summary>
        /// eigener Vergleicher überschrieben Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MSRMSR_GerätSignalSignalKonfig other)
        {
            return (this.CompareTo(other) == 0);
        }
        /// <summary>
        /// Vergleicher überschreibt Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (this.CompareTo((MSRMSR_GerätSignalSignalKonfig)obj) == 0);
        }
        /// <summary>
        /// eigenr Hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// eigener Vergleicher auf Gleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(MSRMSR_GerätSignalSignalKonfig x, MSRMSR_GerätSignalSignalKonfig y)
        {
            return x.Equals(y);
        }
        /// <summary>
        /// eigener Vergleicher auf Ungleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(MSRMSR_GerätSignalSignalKonfig x, MSRMSR_GerätSignalSignalKonfig y)
        {
            return !x.Equals(y);
        }
        // Ende eigner Code
    }
    public partial class MSRMSR_GerätSignalRevision
    {
        // Beginn eigner Code
        /// <summary>
        /// Vergleicher auf Wertebene
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareTo(MSRMSR_GerätSignalRevision y)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream memoryStreamX = new MemoryStream())
            {
                using (MemoryStream memoryStreamY = new MemoryStream())
                {
                    formatter.Serialize(memoryStreamX, this);
                    formatter.Serialize(memoryStreamY, y);

                    var binaryArrayX = memoryStreamX.GetBuffer();
                    var binaryArrayY = memoryStreamY.GetBuffer();

                    if (binaryArrayX.Length != binaryArrayY.Length)
                        return binaryArrayX.Length.CompareTo(binaryArrayY.Length);

                    for (int i = 0; i < binaryArrayX.Length; i++)
                        if (binaryArrayX[i] != binaryArrayY[i])
                            return binaryArrayX[i].CompareTo(binaryArrayY[i]);
                }
            }

            return 0;
        }
        /// <summary>
        /// eigener Vergleicher überschrieben Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MSRMSR_GerätSignalRevision other)
        {
            return (this.CompareTo(other) == 0);
        }
        /// <summary>
        /// Vergleicher überschreibt Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (this.CompareTo((MSRMSR_GerätSignalRevision)obj) == 0);
        }
        /// <summary>
        /// eigenr Hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// eigener Vergleicher auf Gleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(MSRMSR_GerätSignalRevision x, MSRMSR_GerätSignalRevision y)
        {
            return x.Equals(y);
        }
        /// <summary>
        /// eigener Vergleicher auf Ungleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(MSRMSR_GerätSignalRevision x, MSRMSR_GerätSignalRevision y)
        {
            return !x.Equals(y);
        }
        // Ende eigner Code
    }

    public partial class MSRMSR_GerätSignalHW_Anbindung
    {
        // Beginn eigner Code
        /// <summary>
        /// Vergleicher auf Wertebene
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareTo(MSRMSR_GerätSignalHW_Anbindung y)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream memoryStreamX = new MemoryStream())
            {
                using (MemoryStream memoryStreamY = new MemoryStream())
                {
                    formatter.Serialize(memoryStreamX, this);
                    formatter.Serialize(memoryStreamY, y);

                    var binaryArrayX = memoryStreamX.GetBuffer();
                    var binaryArrayY = memoryStreamY.GetBuffer();

                    if (binaryArrayX.Length != binaryArrayY.Length)
                        return binaryArrayX.Length.CompareTo(binaryArrayY.Length);

                    for (int i = 0; i < binaryArrayX.Length; i++)
                        if (binaryArrayX[i] != binaryArrayY[i])
                            return binaryArrayX[i].CompareTo(binaryArrayY[i]);
                }
            }

            return 0;
        }
        /// <summary>
        /// eigener Vergleicher überschrieben Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MSRMSR_GerätSignalHW_Anbindung other)
        {
            return (this.CompareTo(other) == 0);
        }
        /// <summary>
        /// Vergleicher überschreibt Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (this.CompareTo((MSRMSR_GerätSignalHW_Anbindung)obj) == 0);
        }
        /// <summary>
        /// eigenr Hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// eigener Vergleicher auf Gleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(MSRMSR_GerätSignalHW_Anbindung x, MSRMSR_GerätSignalHW_Anbindung y)
        {
            return x.Equals(y);
        }
        /// <summary>
        /// eigener Vergleicher auf Ungleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(MSRMSR_GerätSignalHW_Anbindung x, MSRMSR_GerätSignalHW_Anbindung y)
        {
            return !x.Equals(y);
        }
        // Ende eigner Code
    }

    public partial class MSRMSR_GerätSignalHW_AnbindungProfibus_PA
    {
        // Beginn eigner Code
        /// <summary>
        /// Vergleicher auf Wertebene
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareTo(MSRMSR_GerätSignalHW_AnbindungProfibus_PA y)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream memoryStreamX = new MemoryStream())
            {
                using (MemoryStream memoryStreamY = new MemoryStream())
                {
                    formatter.Serialize(memoryStreamX, this);
                    formatter.Serialize(memoryStreamY, y);

                    var binaryArrayX = memoryStreamX.GetBuffer();
                    var binaryArrayY = memoryStreamY.GetBuffer();

                    if (binaryArrayX.Length != binaryArrayY.Length)
                        return binaryArrayX.Length.CompareTo(binaryArrayY.Length);

                    for (int i = 0; i < binaryArrayX.Length; i++)
                        if (binaryArrayX[i] != binaryArrayY[i])
                            return binaryArrayX[i].CompareTo(binaryArrayY[i]);
                }
            }

            return 0;
        }
        /// <summary>
        /// eigener Vergleicher überschrieben Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MSRMSR_GerätSignalHW_AnbindungProfibus_PA other)
        {
            return (this.CompareTo(other) == 0);
        }
        /// <summary>
        /// Vergleicher überschreibt Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (this.CompareTo((MSRMSR_GerätSignalHW_AnbindungProfibus_PA)obj) == 0);
        }
        /// <summary>
        /// eigenr Hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// eigener Vergleicher auf Gleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(MSRMSR_GerätSignalHW_AnbindungProfibus_PA x, MSRMSR_GerätSignalHW_AnbindungProfibus_PA y)
        {
            return x.Equals(y);
        }
        /// <summary>
        /// eigener Vergleicher auf Ungleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(MSRMSR_GerätSignalHW_AnbindungProfibus_PA x, MSRMSR_GerätSignalHW_AnbindungProfibus_PA y)
        {
            return !x.Equals(y);
        }
        // Ende eigner Code
    }

    public partial class MSRMSR_GerätSignalSoftware
    {
        // Beginn eigner Code
        /// <summary>
        /// Vergleicher auf Wertebene
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareTo(MSRMSR_GerätSignalSoftware y)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream memoryStreamX = new MemoryStream())
            {
                using (MemoryStream memoryStreamY = new MemoryStream())
                {
                    formatter.Serialize(memoryStreamX, this);
                    formatter.Serialize(memoryStreamY, y);

                    var binaryArrayX = memoryStreamX.GetBuffer();
                    var binaryArrayY = memoryStreamY.GetBuffer();

                    if (binaryArrayX.Length != binaryArrayY.Length)
                        return binaryArrayX.Length.CompareTo(binaryArrayY.Length);

                    for (int i = 0; i < binaryArrayX.Length; i++)
                        if (binaryArrayX[i] != binaryArrayY[i])
                            return binaryArrayX[i].CompareTo(binaryArrayY[i]);
                }
            }

            return 0;
        }
        /// <summary>
        /// eigener Vergleicher überschrieben Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MSRMSR_GerätSignalSoftware other)
        {
            return (this.CompareTo(other) == 0);
        }
        /// <summary>
        /// Vergleicher überschreibt Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (this.CompareTo((MSRMSR_GerätSignalSoftware)obj) == 0);
        }
        /// <summary>
        /// eigenr Hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// eigener Vergleicher auf Gleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(MSRMSR_GerätSignalSoftware x, MSRMSR_GerätSignalSoftware y)
        {
            return x.Equals(y);
        }
        /// <summary>
        /// eigener Vergleicher auf Ungleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(MSRMSR_GerätSignalSoftware x, MSRMSR_GerätSignalSoftware y)
        {
            return !x.Equals(y);
        }
        // Ende eigner Code
    }

    //neue Class
    /// <summary>
    /// eigene Vergleicher class
    /// </summary>
    public partial class MSRMSR_Gerät_Vergleicher
    {

        private MSRMSR_Gerät _daten_old;
        /// <summary>
        /// 
        /// </summary>
        public MSRMSR_Gerät Daten_old
        {
            get
            {
                return this._daten_old;
            }
            set
            {
                this._daten_old = value;
            }
        }

        private MSRMSR_Gerät _daten_new;
        /// <summary>
        /// 
        /// </summary>
        public MSRMSR_Gerät Daten_new
        {
            get
            {
                return this._daten_new;
            }
            set
            {
                this._daten_new = value;
            }
        }


        // Beginn eigner Code
        /// <summary>
        /// Vergleicher auf Wertebene
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareTo(MSRMSR_Gerät_Vergleicher y)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream memoryStreamX = new MemoryStream())
            {
                using (MemoryStream memoryStreamY = new MemoryStream())
                {
                    formatter.Serialize(memoryStreamX, this);
                    formatter.Serialize(memoryStreamY, y);

                    var binaryArrayX = memoryStreamX.GetBuffer();
                    var binaryArrayY = memoryStreamY.GetBuffer();

                    if (binaryArrayX.Length != binaryArrayY.Length)
                        return binaryArrayX.Length.CompareTo(binaryArrayY.Length);

                    for (int i = 0; i < binaryArrayX.Length; i++)
                        if (binaryArrayX[i] != binaryArrayY[i])
                            return binaryArrayX[i].CompareTo(binaryArrayY[i]);
                }
            }

            return 0;
        }
        /// <summary>
        /// eigener Vergleicher überschrieben Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MSRMSR_Gerät_Vergleicher other)
        {
            return (this.CompareTo(other) == 0);
        }
        /// <summary>
        /// Vergleicher überschreibt Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (this.CompareTo((MSRMSR_Gerät_Vergleicher)obj) == 0);
        }
        /// <summary>
        /// eigenr Hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// eigener Vergleicher auf Gleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(MSRMSR_Gerät_Vergleicher x, MSRMSR_Gerät_Vergleicher y)
        {
            return x.Equals(y);
        }
        /// <summary>
        /// eigener Vergleicher auf Ungleichheit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(MSRMSR_Gerät_Vergleicher x, MSRMSR_Gerät_Vergleicher y)
        {
            return !x.Equals(y);
        }
        // Ende eigner Code
    }


}
