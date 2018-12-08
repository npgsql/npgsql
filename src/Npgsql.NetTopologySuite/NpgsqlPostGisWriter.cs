using System;
using System.Collections.Generic;
using System.IO;
using GeoAPI.Geometries;
using GeoAPI.IO;
using NetTopologySuite.IO;
using NetTopologySuite.Utilities;

namespace Npgsql.NetTopologySuite
{
    // This is copied from NetTopologySuite.IO.PostGis, fixing some bugs until the fixes are merged upstream
    // https://github.com/NetTopologySuite/NetTopologySuite.IO.PostGis/blob/master/NetTopologySuite.IO.PostGis/PostGisWriter.cs
    class NpgsqlPostGisWriter : IBinaryGeometryWriter
    {
        /// <summary>
        /// The byte order to use when writing geometries.
        /// </summary>
        /// <see cref="ByteOrder"/>
        protected ByteOrder EncodingType;

        private Ordinates _outputOrdinates;

        /// <summary>
        /// Initializes writer with LittleIndian byte order.
        /// </summary>
        public NpgsqlPostGisWriter() :
            this(ByteOrder.LittleEndian)
        { }

        /// <summary>
        /// Initializes writer with the specified byte order.
        /// </summary>
        /// <param name="encodingType">Encoding type</param>
		public NpgsqlPostGisWriter(ByteOrder encodingType)
        {
            EncodingType = encodingType;
            HandleOrdinates = Ordinates.None;
        }

        /// <inheritdoc cref="IGeometryWriter{TSink}.Write(IGeometry)"/>
        public byte[] Write(IGeometry geometry)
        {
            return Write(geometry, HandleOrdinates);
        }

        /// <inheritdoc cref="IGeometryWriter{TSink}.Write(IGeometry, Stream)"/>
        public void Write(IGeometry geometry, Stream stream)
        {
            Write(geometry, HandleOrdinates, stream);
        }

        /// <summary>
        /// Writes a binary encoded PostGIS of the given <paramref name="geometry"/> to to an array of bytes.
        /// </summary>
        /// <param name="geometry">The geometry</param>
        /// <param name="ordinates">The ordinates of each geometry's coordinate. <see cref="Ordinates.XY"/> area always written.</param>
        /// <returns>An array of bytes.</returns>
        private byte[] Write(IGeometry geometry, Ordinates ordinates)
        {
            int coordinateSpace = 8 * OrdinatesUtility.OrdinatesToDimension(ordinates);
            byte[] bytes = GetBytes(geometry, coordinateSpace);
            Write(geometry, ordinates, new MemoryStream(bytes));

            return bytes;
        }



        /// <summary>
        /// Writes a binary encoded PostGIS of the given <paramref name="geometry"/> to <paramref name="stream"/>.
        /// </summary>
        /// <param name="geometry">The geometry</param>
        /// <param name="ordinates">The ordinates of each geometry's coordinate. <see cref="Ordinates.XY"/> area always written.</param>
        /// <param name="stream">The stream to write to</param>
        private void Write(IGeometry geometry, Ordinates ordinates, Stream stream)
        {
            using (var writer = EncodingType == ByteOrder.LittleEndian ? new BinaryWriter(stream) : new BEBinaryWriter(stream))
            {
                Write(geometry, ordinates, EncodingType, writer);
            }
        }

        ///// <summary>
        ///// Writes a binary encoded PostGIS or the given <paramref name="geometry"/> using the provided <paramref name="writer"/>.
        ///// </summary>
        ///// <param name="geometry">The geometry to write.</param>
        ///// <param name="writer">The writer to use.</param>
        //private void Write(IGeometry geometry, BinaryWriter writer)
        //{
        //    Write(geometry, CheckOrdinates(geometry), EncodingType, writer);
        //}

        /// <summary>
        /// Writes a binary encoded PostGIS or the given <paramref name="geometry"/> using the provided <paramref name="writer"/>.
        /// </summary>
        /// <param name="geometry">The geometry to write.</param>
        /// <param name="ordinates">The ordinates of each geometry's coordinate. <see cref="Ordinates.XY"/> area always written.</param>
        /// <param name="byteOrder">The byte order.</param>
        /// <param name="writer">The writer to use.</param>
        private void Write(IGeometry geometry, Ordinates ordinates, ByteOrder byteOrder, BinaryWriter writer)
        {
            if (ordinates == Ordinates.None)
                ordinates = CheckOrdinates(geometry);

            if (geometry is IPoint)
                Write(geometry as IPoint, ordinates, byteOrder, writer);
            else if (geometry is ILinearRing)
                Write(geometry as ILinearRing, ordinates, byteOrder, writer);
            else if (geometry is ILineString)
                Write(geometry as ILineString, ordinates, byteOrder, writer);
            else if (geometry is IPolygon)
                Write(geometry as IPolygon, ordinates, byteOrder, writer);
            else if (geometry is IMultiPoint)
                Write(geometry as IMultiPoint, ordinates, byteOrder, writer);
            else if (geometry is IMultiLineString)
                Write(geometry as IMultiLineString, ordinates, byteOrder, writer);
            else if (geometry is IMultiPolygon)
                Write(geometry as IMultiPolygon, ordinates, byteOrder, writer);
            else if (geometry is IGeometryCollection)
                Write(geometry as IGeometryCollection, ordinates, byteOrder, writer);
            else throw new ArgumentException("Geometry not recognized: " + geometry);
        }

        /// <summary>
        /// Writes the binary encoded PostGIS header.
        /// </summary>
        /// <param name="byteOrder">The byte order specified.</param>
        /// <param name="type">The PostGIS geometry type.</param>
        /// <param name="srid">The spatial reference of the geometry</param>
        /// <param name="ordinates"></param>
        /// <param name="writer">The writer to use.</param>
        private static void WriteHeader(PostGisGeometryType type, int srid, Ordinates ordinates, ByteOrder byteOrder, BinaryWriter writer)
        {
            writer.Write((byte)byteOrder);

            // write typeword
            uint typeword = (uint)type;

            if ((ordinates & Ordinates.Z) != 0)
                typeword |= 0x80000000;

            if ((ordinates & Ordinates.M) != 0)
                typeword |= 0x40000000;

            if (srid != -1)
                typeword |= 0x20000000;
            writer.Write(typeword);

            if (srid != -1)
                writer.Write(srid);
        }

        private static void Write(ICoordinateSequence sequence, Ordinates ordinates, BinaryWriter writer, bool justOne)
        {
            if (sequence == null || sequence.Count == 0)
                return;

            int length = (justOne ? 1 : sequence.Count);

            if (!justOne)
                writer.Write(length);

            for (int i = 0; i < length; i++)
            {
                writer.Write(sequence.GetOrdinate(i, Ordinate.X));
                writer.Write(sequence.GetOrdinate(i, Ordinate.Y));
                if ((ordinates & Ordinates.Z) != 0)
                {
                    double z = sequence.GetOrdinate(i, Ordinate.Z);
                    if (double.IsNaN(z)) z = 0d;
                    writer.Write(z);
                }
                if ((ordinates & Ordinates.M) != 0)
                {
                    double m = sequence.GetOrdinate(i, Ordinate.M);
                    if (double.IsNaN(m)) m = 0d;
                    writer.Write(m);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="byteOrder"></param>
        /// <param name="point"></param>
        /// <param name="ordinates"></param>
        /// <param name="writer"></param>
        private void Write(IPoint point, Ordinates ordinates, ByteOrder byteOrder, BinaryWriter writer)
        {
            WriteHeader(PostGisGeometryType.Point, HandleSRID ? point.SRID : -1, ordinates, byteOrder, writer);
            Write(point.CoordinateSequence, ordinates, writer, true);
        }

        /// <summary>
        /// Write an Array of "full" Geometries
        /// </summary>
        /// <param name="geometries"></param>
        /// <param name="ordinates"></param>
        /// <param name="byteOrder"></param>
        /// <param name="writer"></param>
        private void Write(IList<IGeometry> geometries, Ordinates ordinates, ByteOrder byteOrder, BinaryWriter writer)
        {
            for (int i = 0; i < geometries.Count; i++)
            {
                Write(geometries[i], ordinates, byteOrder, writer);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lineString"></param>
        /// <param name="ordinates"></param>
        /// <param name="byteOrder">The byte order.</param>
        /// <param name="writer"></param>
        private void Write(ILineString lineString, Ordinates ordinates, ByteOrder byteOrder, BinaryWriter writer)
        {
            WriteHeader(PostGisGeometryType.LineString, HandleSRID ? lineString.SRID : -1, ordinates, byteOrder, writer);
            Write(lineString.CoordinateSequence, ordinates, writer, false);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="linearRing"></param>
        /// <param name="ordinates"></param>
        /// <param name="writer"></param>
        private void Write(ILinearRing linearRing, Ordinates ordinates, BinaryWriter writer)
        {
            Write(linearRing.CoordinateSequence, ordinates, writer, false);
        }

        /// <summary>
        /// Writes a 'Polygon' to the stream.
        /// </summary>
        /// <param name="polygon">The polygon to write.</param>
        /// <param name="ordinates">The ordinates to write. <see cref="Ordinates.XY"/> are always written.</param>
        /// <param name="byteOrder">The byte order.</param>
        /// <param name="writer">The writer to use.</param>
        private void Write(IPolygon polygon, Ordinates ordinates, ByteOrder byteOrder, BinaryWriter writer)
        {
            WriteHeader(PostGisGeometryType.Polygon,
                HandleSRID ? polygon.SRID : -1,
                ordinates, byteOrder, writer);
            writer.Write(polygon.NumInteriorRings + 1);
            Write(polygon.ExteriorRing as ILinearRing, ordinates, writer);
            for (int i = 0; i < polygon.NumInteriorRings; i++)
                Write((ILinearRing)polygon.InteriorRings[i], ordinates, writer);
        }

        /// <summary>
        /// Writes a 'MultiPoint' to the stream.
        /// </summary>
        /// <param name="multiPoint">The polygon to write.</param>
        /// <param name="ordinates">The ordinates to write. <see cref="Ordinates.XY"/> are always written.</param>
        /// <param name="byteOrder">The byte order.</param>
        /// <param name="writer">The writer to use.</param>
        private void Write(IMultiPoint multiPoint, Ordinates ordinates, ByteOrder byteOrder, BinaryWriter writer)
        {
            WriteHeader(PostGisGeometryType.MultiPoint,
                HandleSRID ? multiPoint.SRID : -1,
                ordinates, byteOrder, writer);
            writer.Write(multiPoint.NumGeometries);
            Write(multiPoint.Geometries, ordinates, byteOrder, writer);
        }

        /// <summary>
        /// Writes a 'MultiLineString' to the stream.
        /// </summary>
        /// <param name="multiLineString">The polygon to write.</param>
        /// <param name="ordinates">The ordinates to write. <see cref="Ordinates.XY"/> are always written.</param>
        /// <param name="byteOrder">The byte order.</param>
        /// <param name="writer">The writer to use.</param>
        private void Write(IMultiLineString multiLineString, Ordinates ordinates, ByteOrder byteOrder, BinaryWriter writer)
        {
            WriteHeader(PostGisGeometryType.MultiLineString, HandleSRID ? multiLineString.SRID : -1, ordinates, byteOrder, writer);
            writer.Write(multiLineString.NumGeometries);
            Write(multiLineString.Geometries, ordinates, byteOrder, writer);
        }

        /// <summary>
        /// Writes a 'MultiPolygon' to the stream.
        /// </summary>
        /// <param name="multiPolygon">The polygon to write.</param>
        /// <param name="ordinates">The ordinates to write. <see cref="Ordinates.XY"/> are always written.</param>
        /// <param name="byteOrder">The byte order.</param>
        /// <param name="writer">The writer to use.</param>
        private void Write(IMultiPolygon multiPolygon, Ordinates ordinates, ByteOrder byteOrder, BinaryWriter writer)
        {
            WriteHeader(PostGisGeometryType.MultiPolygon, HandleSRID ? multiPolygon.SRID : -1, ordinates, byteOrder, writer);
            writer.Write(multiPolygon.NumGeometries);
            Write(multiPolygon.Geometries, ordinates, byteOrder, writer);
        }

        /// <summary>
        /// Writes a 'GeometryCollection' to the stream.
        /// </summary>
        /// <param name="geomCollection">The polygon to write.</param>
        /// <param name="ordinates">The ordinates to write. <see cref="Ordinates.XY"/> are always written.</param>
        /// <param name="byteOrder">The byte order.</param>
        /// <param name="writer">The writer to use.</param>
        private void Write(IGeometryCollection geomCollection, Ordinates ordinates, ByteOrder byteOrder, BinaryWriter writer)
        {
            WriteHeader(PostGisGeometryType.GeometryCollection, HandleSRID ? geomCollection.SRID : -1, ordinates, byteOrder, writer);
            writer.Write(geomCollection.NumGeometries);
            Write(geomCollection.Geometries, ordinates, byteOrder, writer);
        }

        #region Prepare Buffer
        /// <summary>
        /// Supplies a byte array for the  length for Byte Stream.
        /// </summary>
        /// <param name="geometry">The geometry that needs to be written.</param>
        /// <param name="coordinateSpace">The size that is needed per ordinate.</param>
        /// <returns></returns>
        private byte[] GetBytes(IGeometry geometry, int coordinateSpace)
        {
            return new byte[GetByteStreamSize(geometry, coordinateSpace)];
        }

        /// <summary>
        /// Gets the required size for the byte stream's buffer to hold the geometry information.
        /// </summary>
        /// <param name="geometry">The geometry to write</param>
        /// <param name="coordinateSpace">The size for each ordinate entry.</param>
        /// <returns>The size</returns>
        private int GetByteStreamSize(IGeometry geometry, int coordinateSpace)
        {
            int result = 0;

            // write endian flag
            result += 1;

            // write typeword
            result += 4;

            if (HandleSRID & (geometry.SRID != -1))
                result += 4;

            if (geometry is IPoint)
                result += GetByteStreamSize(geometry as IPoint, coordinateSpace);
            else if (geometry is ILineString)
                result += GetByteStreamSize(geometry as ILineString, coordinateSpace);
            else if (geometry is IPolygon)
                result += GetByteStreamSize(geometry as IPolygon, coordinateSpace);
            else if (geometry is IMultiPoint)
                result += GetByteStreamSize(geometry as IMultiPoint, coordinateSpace);
            else if (geometry is IMultiLineString)
                result += GetByteStreamSize(geometry as IMultiLineString, coordinateSpace);
            else if (geometry is IMultiPolygon)
                result += GetByteStreamSize(geometry as IMultiPolygon, coordinateSpace);
            else if (geometry is IGeometryCollection)
                result += GetByteStreamSize(geometry as IGeometryCollection, coordinateSpace);
            else
                throw new ArgumentException("ShouldNeverReachHere");

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="coordinateSpace">The size needed for each coordinate</param>
        /// <returns></returns>
        private int GetByteStreamSize(IGeometryCollection geometry, int coordinateSpace)
        {
            // 4-byte count + subgeometries
            return 4 + GetByteStreamSize(geometry.Geometries, coordinateSpace);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="coordinateSpace">The size needed for each coordinate</param>
        /// <returns></returns>
        private int GetByteStreamSize(IMultiPolygon geometry, int coordinateSpace)
        {
            // 4-byte count + subgeometries
            return 4 + GetByteStreamSize(geometry.Geometries, coordinateSpace);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="coordinateSpace">The size needed for each coordinate</param>
        /// <returns></returns>
        private int GetByteStreamSize(IMultiLineString geometry, int coordinateSpace)
        {
            // 4-byte count + subgeometries
            return 4 + GetByteStreamSize(geometry.Geometries, coordinateSpace);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="coordinateSpace">The size needed for each coordinate</param>
        /// <returns></returns>
        private int GetByteStreamSize(IMultiPoint geometry, int coordinateSpace)
        {
            // int size
            int result = 4;
            if (geometry.NumPoints > 0)
            {
                // We can shortcut here, as all subgeoms have the same fixed size
                result += geometry.NumPoints * GetByteStreamSize(geometry.GetGeometryN(0), coordinateSpace);
            }
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="coordinateSpace">The size needed for each coordinate</param>
        /// <returns></returns>
        private int GetByteStreamSize(IPolygon geometry, int coordinateSpace)
        {
            // int length
            int result = 4;
            result += GetByteStreamSize(geometry.ExteriorRing, coordinateSpace);
            for (int i = 0; i < geometry.NumInteriorRings; i++)
                result += GetByteStreamSize(geometry.InteriorRings[i], coordinateSpace);
            return result;
        }

        /// <summary>
        /// Write an Array of "full" Geometries
        /// </summary>
        /// <param name="container"></param>
        /// <param name="coordinateSpace">The size needed for each coordinate</param>
        /// <returns></returns>
        private int GetByteStreamSize(IList<IGeometry> container, int coordinateSpace)
        {
            int result = 0;
            for (int i = 0; i < container.Count; i++)
            {
                result += GetByteStreamSize(container[i], coordinateSpace);
            }
            return result;
        }

        /// <summary>
        /// Calculates the amount of space needed to write this coordinate sequence.
        /// </summary>
        /// <param name="sequence">The sequence</param>
        /// <param name="coordinateSpace">The size needed for each coordinate</param>
        /// <returns></returns>
        private static int GetByteStreamSize(ICoordinateSequence sequence, int coordinateSpace)
        {
            // number of points
            const int result = 4;

            // And the amount of the points itsself, in consistent geometries
            // all points have equal size.
            if (sequence.Count == 0)
                return result;

            return result + sequence.Count * coordinateSpace;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="coordinateSpace">The size needed for each coordinate</param>
        /// <returns></returns>
        protected int GetByteStreamSize(ILineString geometry, int coordinateSpace)
        {
            return GetByteStreamSize(geometry.CoordinateSequence, coordinateSpace);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="coordinateSpace">The size needed for each coordinate</param>
        /// <returns></returns>
        protected int GetByteStreamSize(ILinearRing geometry, int coordinateSpace)
        {
            return GetByteStreamSize(geometry.CoordinateSequence, coordinateSpace);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="coordinateSpace">The size needed for each coordinate</param>
        /// <returns></returns>
        protected int GetByteStreamSize(IPoint geometry, int coordinateSpace)
        {
            return coordinateSpace;
        }

        #endregion

        #region Check ordinates

        private static Ordinates CheckOrdinates(IGeometry geometry)
        {
            if (geometry is IPoint)
                return CheckOrdinates(((IPoint)geometry).CoordinateSequence);
            if (geometry is ILineString)
                return CheckOrdinates(((ILineString)geometry).CoordinateSequence);
            if (geometry is IPolygon)
                return CheckOrdinates((((IPolygon)geometry).ExteriorRing).CoordinateSequence);
            if (geometry is IGeometryCollection collection)
                return collection.Count == 0 ? Ordinates.None : CheckOrdinates(collection.GetGeometryN(0));

            Assert.ShouldNeverReachHere();
            return Ordinates.None;

        }

        private static Ordinates CheckOrdinates(ICoordinateSequence sequence)
        {
            if (sequence == null || sequence.Count == 0)
                return Ordinates.None;

            var result = Ordinates.XY;
            if ((sequence.Ordinates & Ordinates.Z) != 0)
            {
                if (!double.IsNaN(sequence.GetOrdinate(0, Ordinate.Z)))
                    result |= Ordinates.Z;
            }
            if ((sequence.Ordinates & Ordinates.M) != 0)
            {
                // Fixed in Npgsql, see https://github.com/NetTopologySuite/NetTopologySuite.IO.PostGis/pull/4
                if (!double.IsNaN(sequence.GetOrdinate(0, Ordinate.M)))
                    result |= Ordinates.M;
            }
            return result;
        }

        #endregion

        #region Implementation of IGeometryIOSettings

        /// <inheritdoc cref="IGeometryIOSettings.HandleSRID"/>
        public bool HandleSRID { get { return true; } set { } }

        /// <inheritdoc cref="IGeometryIOSettings.AllowedOrdinates"/>
        public Ordinates AllowedOrdinates
        {
            get { return Ordinates.XYZM; }
        }

        /// <inheritdoc cref="IGeometryIOSettings.HandleOrdinates"/>
        public Ordinates HandleOrdinates
        {
            get => _outputOrdinates;
            // Fixed in Npgsql, see https://github.com/NetTopologySuite/NetTopologySuite.IO.PostGis/pull/5
            set => _outputOrdinates = value == Ordinates.None
                ? Ordinates.None
                : (value | Ordinates.XY) & AllowedOrdinates;
        }
        #endregion

        #region Implementation of IBinaryGeometryWriter


        /// <inheritdoc cref="IBinaryGeometryWriter.ByteOrder"/>
        public ByteOrder ByteOrder
        {
            get { return EncodingType; }
            set { throw new InvalidOperationException(); }
        }

        #endregion
    }
}
