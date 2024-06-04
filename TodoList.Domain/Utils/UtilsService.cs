namespace TodoList.Domain.Utils
{
    public static class UtilsService
    {
        /// <summary>
        /// Generates a Comb-style GUID with timestamp information.
        /// </summary>
        public class CombGenerator
        {
            /// <summary>
            /// Generates a Comb-style GUID using timestamp and random data.
            /// </summary>
            /// <returns>A Comb-style GUID.</returns>
            public static Guid GenerateComb()
            {
                byte[] guidArray = Guid.NewGuid().ToByteArray();

                DateTime baseDate = new(1900, 1, 1);
                DateTime now = DateTime.Now;

                // Get the days and milliseconds which will be used to build the byte string 
                TimeSpan days = new(now.Ticks - baseDate.Ticks);
                TimeSpan msecs = now.TimeOfDay;

                // Convert to a byte array 
                // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
                byte[] daysArray = BitConverter.GetBytes(days.Days);
                byte[] msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

                // Reverse the bytes to match SQL Servers ordering 
                Array.Reverse(daysArray);
                Array.Reverse(msecsArray);

                // Copy the bytes into the guid 
                Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
                Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

                return new Guid(guidArray);
            }
        }
    }
}
