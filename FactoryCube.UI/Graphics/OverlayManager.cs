using FactoryCube.UI.ViewModels;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryCube.UI.Graphics
{
    public class OverlayManager
    {
        private HWindow _window;
        private int _imageWidth;
        private int _imageHeight;

        public bool ShowCrosshair { get; set; } = true;
        public bool ShowGrid { get; set; } = false;

        public int GridSpacing { get; set; } = 100; // pixels
        private readonly List<(double X, double Y, double Width, double Height)> _rectangles = new();

        public OverlayManager(HWindow window, int imageWidth, int imageHeight)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _imageWidth = imageWidth;
            _imageHeight = imageHeight;
        }

        private readonly List<List<(double X, double Y)>> _polygons = new();
        private List<(double X, double Y)> _currentPolygon = new();

        public void AddCenteredRectangle(double centerX, double centerY, double width, double height)
        {
            _rectangles.Add((centerX, centerY, width, height));
        }

        public void AddCenteredRectangle(double x, double y)
        {
            double defaultWidth = 80;
            double defaultHeight = 80;
            _rectangles.Add((x, y, defaultWidth, defaultHeight));
        }

        public void AddCenteredEllipse(double x, double y, double rx = 50, double ry = 30)
                        => _ellipses.Add((x, y, rx, ry));
        public void AddPolygonPoint(double x, double y)
        {
            _currentPolygon.Add((x, y));
            if (_currentPolygon.Count > 2)
            {
                _polygons.Add(new List<(double X, double Y)>(_currentPolygon));
                _currentPolygon.Clear();
            }
        }


        private List<(double X, double Y)> _persistentRois = new();

        public void AddCenteredRoi(double x, double y)
        {
            _persistentRois.Add((x, y));
        }

        private void DrawPersistentRois()
        {
            _window.SetColor("red");
            _window.SetLineWidth(2);

            foreach (var (x, y) in _persistentRois)
            {
                double r = 50;
                _window.DispRectangle1(y - r, x - r, y + r, x + r);
            }
        }

        public void ClearAllRois() => _persistentRois.Clear();



        public void UpdateImageSize(int width, int height)
        {
            _imageWidth = width;
            _imageHeight = height;
        }

        public void FinishCurrentPolygon()
        {
            if (_currentPolygon.Count >= 3)
            {
                _polygons.Add(new List<(double X, double Y)>(_currentPolygon));
            }

            _currentPolygon.Clear();
        }


        private void DrawPersistentShapes()
        {
            _window.SetLineWidth(2);

            foreach (var (x, y, width, height) in _rectangles)
            {
                _window.DispRectangle1(y - height / 2, x - width / 2, y + height / 2, x + width / 2);
            }


            foreach (var (x, y, rx, ry) in _ellipses)
                _window.DispEllipse(y, x, 0, ry, rx);

            foreach (var polygon in _polygons)
            {
                for (int i = 0; i < polygon.Count; i++)
                {
                    var (x1, y1) = polygon[i];
                    var (x2, y2) = polygon[(i + 1) % polygon.Count];
                    _window.DispLine(y1, x1, y2, x2);
                }
            }
        }

        private void DrawCrosshair(HWindow window)
        {
            var rowCenter = _imageHeight / 2.0;
            var colCenter = _imageWidth / 2.0;

            window.DispLine(rowCenter, 0, rowCenter, _imageWidth);   // Horizontal
            window.DispLine(0, colCenter, _imageHeight, colCenter); // Vertical
        }

        private void DrawGrid(HWindow window)
        {
            for (int x = GridSpacing; x < _imageWidth; x += GridSpacing)
            {
                window.DispLine(0.0, (double)x, (double)_imageHeight, (double)x);
            }

            for (int y = GridSpacing; y < _imageHeight; y += GridSpacing)
            {
                window.DispLine((double)y, 0.0, (double)y, (double)_imageWidth);
            }
        }

        private void DrawPreviewEllipse()
        {
            if (_previewEllipse is (var x1, var y1, var x2, var y2))
            {
                _window.SetDraw("margin");
                _window.SetLineWidth(2);
                _window.SetColor("orange");

                // compute center & radii
                double cx = (x1 + x2) / 2;
                double cy = (y1 + y2) / 2;
                double rx = Math.Abs(x2 - x1) / 2;
                double ry = Math.Abs(y2 - y1) / 2;

                // note: DispEllipse takes (rowCenter, colCenter, angle, radiusY, radiusX)
                _window.DispEllipse(cy, cx, 0, ry, rx);
            }
        }

        public void DrawSnapCenterRoi(double centerX, double centerY, double radius = 50)
        {
            if (_window == null) return;

            double row1 = centerY - radius;
            double col1 = centerX - radius;
            double row2 = centerY + radius;
            double col2 = centerX + radius;

            _window.SetColor("red");
            _window.SetLineWidth(2);
            _window.DispRectangle1(row1, col1, row2, col2);
        }

        // --- Step 1: Update OverlayManager.cs ---
        private (double X1, double Y1, double X2, double Y2)? _previewRectangle = null;
        private bool _previewSnapSquare = false;

        public void SetPreviewRectangle(double x1, double y1, double x2, double y2, bool snapToSquare)
        {
            if (snapToSquare)
            {
                double width = Math.Abs(x2 - x1);
                double height = Math.Abs(y2 - y1);
                double side = Math.Min(width, height);

                x2 = x1 + Math.Sign(x2 - x1) * side;
                y2 = y1 + Math.Sign(y2 - y1) * side;
            }

            _previewRectangle = (x1, y1, x2, y2);
            _previewSnapSquare = snapToSquare;
        }

        public void ClearPreview() => _previewRectangle = null;

        private void DrawPreview()
        {
            if (_previewRectangle is null) return;

            var (x1, y1, x2, y2) = _previewRectangle.Value;
            _window.SetLineWidth(2);
            _window.SetColor("orange");
            _window.DispRectangle1(y1, x1, y2, x2);
        }

        private (double X, double Y)? _previewPolygonPoint;

        // expose setter
        public void SetPreviewPolygonPoint(double x, double y)
            => _previewPolygonPoint = (x, y);
        public void ClearPreviewPolygon()
            => _previewPolygonPoint = null;

        public void DrawOverlays(IEnumerable<RoiItem> placedRois)
        {
            _window.SetDraw("margin");
            _window.SetLineWidth(1);
            _window.SetColor("lime green");

            if (ShowCrosshair)
                DrawCrosshair(_window);

            if (ShowGrid)
                DrawGrid(_window);

            DrawPersistentShapes();
            DrawPreview();
            DrawPreviewEllipse();

            _window.SetColor("red");
            foreach (var poly in _polygons) { /* loop & DispLine for each edge */ }

            // draw the rubber-band
            if (_previewPolygonPoint.HasValue && _currentPolygon.Count > 0)
            {
                var (lx, ly) = _currentPolygon.Last();
                var (px, py) = _previewPolygonPoint.Value;
                _window.SetColor("orange");
                _window.SetLineWidth(2);
                _window.DispLine(ly, lx, py, px);
            }

            foreach (var roi in placedRois)
            {
                switch (roi)
                {
                    case RectangleRoiItem r:
                        _window.DispRectangle1(r.CenterY - r.Height / 2, r.CenterX - r.Width / 2,
                                              r.CenterY + r.Height / 2, r.CenterX + r.Width / 2);
                        break;
                        // Ellipse, Polygon...
                }
            }
        }

        public void AddRectangleFromCorners(double x1, double y1, double x2, double y2)
        {
            double width = Math.Abs(x2 - x1);
            double height = Math.Abs(y2 - y1);
            double centerX = (x1 + x2) / 2;
            double centerY = (y1 + y2) / 2;

            _rectangles.Add((centerX, centerY, width, height));
        }

        private (double X1, double Y1, double X2, double Y2)? _previewEllipse;

        // Committed ellipses stored as (centerX, centerY, radiusX, radiusY)
        private readonly List<(double X, double Y, double Rx, double Ry)> _ellipses
            = new();

        // … other code …

        /// <summary>
        /// Begins displaying a live-preview ellipse
        /// defined by the two corner points.
        /// </summary>
        public void SetPreviewEllipse(double x1, double y1, double x2, double y2, bool snapToCircle)
        {
            if (snapToCircle)
            {
                double dx = x2 - x1, dy = y2 - y1;
                double side = Math.Min(Math.Abs(dx), Math.Abs(dy));
                x2 = x1 + Math.Sign(dx) * side;
                y2 = y1 + Math.Sign(dy) * side;
            }
            _previewEllipse = (x1, y1, x2, y2);
        }

        /// <summary>
        /// Clears any in-progress ellipse preview.
        /// </summary>
        public void ClearPreviewEllipse()
            => _previewEllipse = null;

        /// <summary>
        /// Commits an ellipse from two corner clicks into the persistent list.
        /// </summary>
        public void AddEllipseFromCorners(double x1, double y1, double x2, double y2, bool snapToCircle)
        {
            if (snapToCircle)
            {
                double dx = x2 - x1, dy = y2 - y1;
                double side = Math.Min(Math.Abs(dx), Math.Abs(dy));
                x2 = x1 + Math.Sign(dx) * side;
                y2 = y1 + Math.Sign(dy) * side;
            }

            double cx = (x1 + x2) / 2;
            double cy = (y1 + y2) / 2;
            double rx = Math.Abs(x2 - x1) / 2;
            double ry = Math.Abs(y2 - y1) / 2;

            _ellipses.Add((cx, cy, rx, ry));
        }
    }
}
