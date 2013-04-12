﻿// XAML Map Control - http://xamlmapcontrol.codeplex.com/
// Copyright © 2013 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

#if NETFX_CORE
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows;
using System.Windows.Media;
#endif

namespace MapControl
{
    /// <summary>
    /// Base class for map shapes.
    /// </summary>
    public partial class MapShape : IMapElement
    {
        private MapBase parentMap;

        public MapBase ParentMap
        {
            get { return parentMap; }
            set
            {
                parentMap = value;
                UpdateGeometry();
            }
        }

        protected readonly Geometry Geometry;

        protected virtual void UpdateGeometry()
        {
        }

        protected override Size MeasureOverride(Size constraint)
        {
            // Shape.MeasureOverride in WPF and WinRT sometimes return a Size with zero
            // width or height, whereas Shape.MeasureOverride in Silverlight occasionally
            // throws an ArgumentException, as it tries to create a Size from a negative
            // width or height, apparently resulting from a transformed Geometry.
            // In either case it seems to be sufficient to simply return a non-zero size.
            return new Size(1, 1);
        }
    }
}
