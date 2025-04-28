namespace App.Utils
{
    public static class CMath
    {

        //----Adn-----//
        public static decimal ToRoundDown(this decimal o, int to)
        {
            return o - o % to;
        }

        public static decimal ToRoundUp(this decimal o, int to)
        {
            return o + to - o % to;
        }

        //----Smp-----//
        public static decimal ToRoundDown25(this decimal o)
        {
            return o - o % 25;
        }

        public static decimal ToRoundDown50(this decimal o)
        {
            return o - o % 50;
        }

        public static decimal ToRoundDown100(this decimal o)
        {
            return o - o % 100;
        }

        public static decimal ToRoundDown1000(this decimal o)
        {
            return o - o % 1000;
        }

        public static decimal ToRoundUp25(this decimal o)
        {
            return o + 25 - o % 25;
        }

        public static decimal ToRoundUp50(this decimal o)
        {
            return o + 50 - o % 50;
        }

        public static decimal ToRoundUp100(this decimal o)
        {
            return o + 100 - o % 100;
        }

        public static decimal ToRoundUp1000(this decimal o)
        {
            return o + 1000 - o % 1000;
        }
    }
}
