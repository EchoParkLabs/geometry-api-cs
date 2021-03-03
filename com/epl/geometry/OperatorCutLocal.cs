/*
Copyright 2017-2021 David Raleigh

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

For additional information, contact:

email: davidraleigh@gmail.com
*/


namespace com.epl.geometry
{
	internal class OperatorCutLocal : com.epl.geometry.OperatorCut
	{
		public abstract class Side
		{
			public const int Left = 0;

			public const int Right = 1;

			public const int Coincident = 2;

			public const int Undefined = 3;

			public const int Uncut = 4;
		}

		public static class SideConstants
		{
		}

		public class CutPair
		{
			public CutPair(com.epl.geometry.Geometry geometry, int side, int ipartCuttee, int ivertexCuttee, double scalarCuttee, int sidePrev, int ipartCutteePrev, int ivertexCutteePrev, double scalarCutteePrev, int ipartCutter, int ivertexCutter, double scalarCutter, int ipartCutterPrev, int ivertexCutterPrev
				, double scalarCutterPrev)
			{
				m_geometry = geometry;
				m_side = side;
				m_ipartCuttee = ipartCuttee;
				m_ivertexCuttee = ivertexCuttee;
				m_scalarCuttee = scalarCuttee;
				m_sidePrev = sidePrev;
				m_ipartCutteePrev = ipartCutteePrev;
				m_ivertexCutteePrev = ivertexCutteePrev;
				m_scalarCutteePrev = scalarCutteePrev;
				m_ipartCutter = ipartCutter;
				m_ivertexCutter = ivertexCutter;
				m_scalarCutter = scalarCutter;
				m_ipartCutterPrev = ipartCutterPrev;
				m_ivertexCutterPrev = ivertexCutterPrev;
				m_scalarCutterPrev = scalarCutterPrev;
			}

			internal com.epl.geometry.Geometry m_geometry;

			internal int m_side;

			internal int m_ipartCuttee;

			internal int m_ivertexCuttee;

			internal double m_scalarCuttee;

			internal int m_sidePrev;

			internal int m_ipartCutteePrev;

			internal int m_ivertexCutteePrev;

			internal double m_scalarCutteePrev;

			internal int m_ipartCutter;

			internal int m_ivertexCutter;

			internal double m_scalarCutter;

			internal int m_ipartCutterPrev;

			internal int m_ivertexCutterPrev;

			internal double m_scalarCutterPrev;
		}

		public override com.epl.geometry.GeometryCursor Execute(bool bConsiderTouch, com.epl.geometry.Geometry cuttee, com.epl.geometry.Polyline cutter, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.ProgressTracker progressTracker)
		{
			return new com.epl.geometry.OperatorCutCursor(bConsiderTouch, cuttee, cutter, spatialReference, progressTracker);
		}
	}
}
