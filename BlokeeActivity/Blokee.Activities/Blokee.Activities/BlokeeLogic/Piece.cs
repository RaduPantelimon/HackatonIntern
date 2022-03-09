using System;

namespace Blokee
{

    public abstract class Piece
    {
        // Properties
        public abstract int Id { get; }
        public abstract int Size { get; }
        public abstract int Weight { get; }
        public bool IsAvailable { get; set; }
        public abstract int[][] Points { get; }
        public abstract int[] Orientations { get; }
        public abstract int[][][] AllVariations { get; }

        protected Piece() : this(false) { }

        public Piece(bool available)
        {
            this.IsAvailable = available;
        }
    }

    public class Piece0 : Piece
    {
        public override int Id => 0;
        public override int Size => this.Points.Length;
        public override int Weight => 40;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 0, 2 }, new int[] { 0, 3 } };
        public override int[] Orientations => new int[] { 0, 1 };
        public override int[][][] AllVariations => throw new NotImplementedException();

        private Piece0() : this(false) { }
        public Piece0(bool available) : base(available) { }
    }

    public class Piece1 : Piece
    {
        public override int Id => 1;
        public override int Size => this.Points.Length;
        public override int Weight => 50;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 0, 2 }, new int[] { 0, 3 }, new int[] { 1, 3 } };
        public override int[] Orientations => new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece1() : this(false) { }
        public Piece1(bool available) : base(available) { }
    }

    public class Piece2 : Piece
    {
        public override int Id => 2;
        public override int Size => this.Points.Length;
        public override int Weight => 50;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 0, 2 }, new int[] { 0, 3 }, new int[] { 1, 2 } };
        public override int[] Orientations => new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece2() : this(false) { }
        public Piece2(bool available) : base(available) { }
    }

    public class Piece3 : Piece
    {
        public override int Id => 3;
        public override int Size => this.Points.Length;
        public override int Weight => 40;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 2, 0 }, new int[] { 2, 1 } };
        public override int[] Orientations => new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece3() : this(false) { }
        public Piece3(bool available) : base(available) { }
    }

    public class Piece4 : Piece
    {
        public override int Id => 4;
        public override int Size => this.Points.Length;
        public override int Weight => 50;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 2, 0 } };
        public override int[] Orientations => new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece4() : this(false) { }
        public Piece4(bool available) : base(available) { }
    }

    public class Piece5 : Piece
    {
        public override int Id => 5;
        public override int Size => this.Points.Length;
        public override int Weight => 50;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 2, 0 }, new int[] { 2, 1 }, new int[] { 2, 2 } };
        public override int[] Orientations => new int[] { 0, 1, 2, 3 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece5() : this(false) { }
        public Piece5(bool available) : base(available) { }
    }

    public class Piece6 : Piece
    {
        public override int Id => 6;
        public override int Size => this.Points.Length;
        public override int Weight => 50;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { 2, 1 }, new int[] { 2, 2 } };
        public override int[] Orientations => new int[] { 0, 1, 4, 5 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece6() : this(false) { }
        public Piece6(bool available) : base(available) { }
    }

    public class Piece7 : Piece
    {
        public override int Id => 7;
        public override int Size => this.Points.Length;
        public override int Weight => 50;
        public override int[][] Points => new int[][] { new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 1, 2 }, new int[] { 2, 1 } };
        public override int[] Orientations => new int[] { 0 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece7() : this(false) { }
        public Piece7(bool available) : base(available) { }
    }

    public class Piece8 : Piece
    {
        public override int Id => 8;
        public override int Size => this.Points.Length;
        public override int Weight => 40;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 2, 0 } };
        public override int[] Orientations => new int[] { 0, 1, 2, 3 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece8() : this(false) { }
        public Piece8(bool available) : base(available) { }
    }

    public class Piece9 : Piece
    {
        public override int Id => 9;
        public override int Size => this.Points.Length;
        public override int Weight => 50;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 2, 0 }, new int[] { 2, 1 } };
        public override int[] Orientations => new int[] { 0, 1, 2, 3 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece9() : this(false) { }
        public Piece9(bool available) : base(available) { }
    }

    public class Piece10 : Piece
    {
        public override int Id => 10;
        public override int Size => this.Points.Length;
        public override int Weight => 50;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 2, 1 }, new int[] { 2, 2 } };
        public override int[] Orientations => new int[] { 0, 1, 2, 3 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece10() : this(false) { }
        public Piece10(bool available) : base(available) { }
    }

    public class Piece11 : Piece
    {
        public override int Id => 11;
        public override int Size => this.Points.Length;
        public override int Weight => 50;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { 2, 1 }, new int[] { 0, 2 } };
        public override int[] Orientations => new int[] { 0, 1, 2, 3 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece11() : this(false) { }
        public Piece11(bool available) : base(available) { }
    }

    public class Piece12 : Piece
    {
        public override int Id => 12;
        public override int Size => this.Points.Length;
        public override int Weight => 50;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { 1, 2 }, new int[] { 2, 1 } };
        public override int[] Orientations => new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece12() : this(false) { }
        public Piece12(bool available) : base(available) { }
    }

    public class Piece13 : Piece
    {
        public override int Id => 13;
        public override int Size => this.Points.Length;
        public override int Weight => 50;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 0, 2 }, new int[] { 1, 2 }, new int[] { 1, 3 } };
        public override int[] Orientations => new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece13() : this(false) { }
        public Piece13(bool available) : base(available) { }
    }

    public class Piece14 : Piece
    {
        public override int Id => 14;
        public override int Size => this.Points.Length;
        public override int Weight => 40;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { 1, 2 } };
        public override int[] Orientations => new int[] { 0, 1, 4, 5 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece14() : this(false) { }
        public Piece14(bool available) : base(available) { }
    }

    public class Piece15 : Piece
    {
        public override int Id => 15;
        public override int Size => this.Points.Length;
        public override int Weight => 30;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 1, 1 } };
        public override int[] Orientations => new int[] { 0, 1, 2, 3 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece15() : this(false) { }
        public Piece15(bool available) : base(available) { }
    }

    public class Piece16 : Piece
    {
        public override int Id => 16;
        public override int Size => this.Points.Length;
        public override int Weight => 40;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 1, 1 } };
        public override int[] Orientations => new int[] { 0 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece16() : this(false) { }
        public Piece16(bool available) : base(available) { }
    }

    public class Piece17 : Piece
    {
        public override int Id => 17;
        public override int Size => this.Points.Length;
        public override int Weight => 10;
        public override int[][] Points => new int[][] { new int[] { 0, 0 } };
        public override int[] Orientations => new int[] { 0 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece17() : this(false) { }
        public Piece17(bool available) : base(available) { }
    }

    public class Piece18 : Piece
    {
        public override int Id => 18;
        public override int Size => this.Points.Length;
        public override int Weight => 50;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 0, 2 }, new int[] { 0, 3 }, new int[] { 0, 4 } };
        public override int[] Orientations => new int[] { 0, 1 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece18() : this(false) { }
        public Piece18(bool available) : base(available) { }
    }

    public class Piece19 : Piece
    {
        public override int Id => 19;
        public override int Size => this.Points.Length;
        public override int Weight => 30;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 0, 2 } };
        public override int[] Orientations => new int[] { 0, 1 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece19() : this(false) { }
        public Piece19(bool available) : base(available) { }
    }

    public class Piece20 : Piece
    {
        public override int Id => 20;
        public override int Size => this.Points.Length;
        public override int Weight => 20;
        public override int[][] Points => new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 } };
        public override int[] Orientations => new int[] { 0, 1 };
        public override int[][][] AllVariations => throw new NotImplementedException();
        private Piece20() : this(false) { }
        public Piece20(bool available) : base(available) { }
    }
}
