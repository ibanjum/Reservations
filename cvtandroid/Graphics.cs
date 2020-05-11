using System;
using SkiaSharp.Views.Forms;
using Constructivity.Core;
using Xamarin.Forms;
using SkiaSharp;
using System.Text;

namespace cvtandroid
{
    public class Graphics
    {

        public static SKCanvas CreateSVG(object graph, SKCanvas canvas, SKImageInfo info, double wx, double wy)
        {
            string svgpath = null;
            double xmin = 0.0;
            double xmax = 0.0;
            double ymin = 0.0;
            double ymax = 0.0;

            if (graph.GetType() == typeof(Material))
            {
                Material material = graph as Material;
                if (material == null)
                    return null;

                return CreateSVG(material, canvas, info);
            }
            if(graph.GetType() == typeof(Construction))
            {
                Construction con = graph as Construction;
                if (con == null)
                    return null;

                return CreateSVG(con, canvas, info, wx, wy);
            }
            if(graph.GetType() == typeof(Section))
            {
                Section section = graph as Section;
                if (section == null)
                    return null;

                return CreateSVG(section, canvas, info, wx, wy);
            }
            if(graph.GetType() == typeof(Contour))
            {
                Contour contour = graph as Contour;
                if (contour == null)
                    return null;

                if (contour.Segments == null || contour.Segments.Length == 0)
                    return null;

                svgpath = LayoutSVG(contour, out xmin, out ymin, out xmax, out ymax);
                return TransformSVG(svgpath, canvas, info, xmin, ymin, xmax, ymax, wx, wy);
            }
            if(graph.GetType() == typeof(Space))
            {
                Space space = graph as Space;
                if (space == null)
                    return null;

                return CreateSVG(space, canvas, info, wx, wy);
            }
            return null;
        }
        public static SKCanvas CreateSVG(Space space, SKCanvas canvas, SKImageInfo info, double wx, double wy)
        {
            double xmin = 0.0;
            double ymin = 0.0;
            double xmax = 0.0;
            double ymax = 0.0;

            string svgpath = LayoutSVG(space.X, space.Y, space.Segments, out xmin, out ymin, out xmax, out ymax);

            return TransformSVG(svgpath, canvas, info, xmin, ymin, xmax, ymax, wx, wy);
        }
        public static SKCanvas CreateSVG(Construction con, SKCanvas canvas, SKImageInfo info, double wx, double wy)
        {
            StringBuilder svgpath = new StringBuilder();
            double xmin = 0.0;
            double xmax = 0.0;
            double ymin = 0.0;
            double ymax = 0.0;

            if (con.Layers != null)
            {
                float y = 0;

                foreach (Layer layer in con.Layers)
                {
                    float t = (float)layer.Thickness.GetValueOrDefault();

                    if (layer.Thickness != null && layer.Members == null)
                    {
                        SKPaint paintMaterial = new SKPaint();
                        if (layer.Material != null)
                        {
                            paintMaterial.Style = SKPaintStyle.Fill;
                            paintMaterial.Color = ConvertColor(layer.Material.Color).ToSKColor();
                        }

                        
                        SKPaint paintStroke = new SKPaint()
                        {
                            Style = SKPaintStyle.Stroke,
                            Color = SKColors.Black,
                            StrokeWidth = 1,
                        };

                        canvas.DrawRect(0, (float)(y * wy), (float)wx, (float)(t * wy), paintMaterial);
                        canvas.DrawRect(0, (float)(y * wy), (float)wx, (float)(t * wy), paintStroke);
                    }

                    if (layer.Members != null)
                    {
                        // overlay on top of layer material, if any
                        int i = 0;
                        foreach (Member member in layer.Members)
                        {
                            if (member.Perpendicular &&
                                member.Spacing.GetValueOrDefault() > 0.0 &&
                                member.Section != null)
                            {
                                Section section = member.Section;

                                for (float x = (float)member.Offset.GetValueOrDefault(); x < 1.0; x += (float)member.Spacing.GetValueOrDefault())
                                {

                                    if (section.Segments != null)
                                    {
                                        string _svgpath = LayoutSVG(x * wx, y * wy, section.Segments, out xmin, out ymin, out xmax, out ymax);
                                        svgpath.Append(svgpath);
                                    }
                                    else if (section.LengthX != null && section.LengthY != null)
                                    {
                                        float sx = (float)(section.LengthX.Value * wx);
                                        float sy = (float)(section.LengthY.Value * wy);

       
                                        SKPaint paintSection = new SKPaint();
                                        if (section.Material != null && section.Material.Color != 0)
                                        {
                                            paintSection.Style = SKPaintStyle.Fill;
                                            paintSection.Color = ConvertColor(section.Material.Color).ToSKColor();
                                        }

                                        SKPaint paintStroke = new SKPaint()
                                        {
                                            Style = SKPaintStyle.Stroke,
                                            StrokeWidth = 1,
                                            Color = SKColors.Black,
                                        };

                                        canvas.DrawRect((float)(x * wx), (float)(y * wy), sx, sy, paintSection);
                                        canvas.DrawRect((float)(x * wx), (float)(y * wy), sx, sy, paintStroke);

                                        canvas.DrawLine((float)(x * wx), (float)(y * wy), (float)(x * wx + sx), (float)(y * wy + sy), paintStroke);
                                        canvas.DrawLine((float)(x * wx), (float)(y * wy + sy), (float)(x * wx + sx), (float)(y * wy), paintStroke);
                                        //sb.Append("<rect x=\"" + x * wx + "\" y=\"" + y * wy + "\" width=\"" + sx + "\" height=\"" + sy + "\" style=\"fill:rgb(" + r + "," + g + "," + b + ");stroke-width:1;stroke:rgb(0,0,0)\" />");
                                        //sb.Append("<line X1=\"" + x * wx + "\" Y1=\"" + (y * wy) + "\" X2=\"" + (x * wx + sx) + "\" Y2=\"" + (y * wy + sy) + "\" style=\"stroke-width:1;stroke:rgb(0,0,0)\" />");
                                       // sb.Append("<line X1=\"" + x * wx + "\" Y1=\"" + (y * wy + sy) + "\" X2=\"" + (x * wx + sx) + "\" Y2=\"" + (y * wy) + "\" style=\"stroke-width:1;stroke:rgb(0,0,0)\" />");
                                    }

                                    i++;

                                    if (i > 32)
                                        break; // sanity check
                                }
                            }
                        }
                    }
                    y += t;
                }
            }
            return TransformSVG(svgpath.ToString(), canvas, info, xmin, ymin, xmax, ymax, wx, wy);
        }
        public static SKCanvas CreateSVG(Material material, SKCanvas canvas, SKImageInfo info)
        {
            SKPaint paint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = ConvertColor(material.Color).ToSKColor(),
            };
            canvas.DrawRect(info.Width / 2, info.Height / 2, 100, 100, paint);
            return canvas;
        }
        public static SKCanvas CreateSVG(Section section, SKCanvas canvas, SKImageInfo info, double wx, double wy)
        {

            SKPaint paint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.White,
            };
            SKPaint painoutline = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Black,
            };

            double xmin = 0.0;
            double ymin = 0.0;
            double xmax = 0.0;
            double ymax = 0.0;
            string svgpath = null;

            if (section.Segments != null && section.Segments.Length >= 3)
            {
                // render outer contour
                svgpath = LayoutSVG(0.0, 0.0, section.Segments, out xmin, out ymin, out xmax, out ymax);
            }
            else if (section.LengthX != null && section.LengthY != null)
            {

            }
            else if (section.LengthX != null && section.ThicknessX != null)
            {
                // hollow circle
                double factor = section.ThicknessX.GetValueOrDefault() / section.LengthX.GetValueOrDefault();

                float r = (float)(100.0 * factor);

                canvas.DrawCircle(100, 100, 100, paint);
                canvas.DrawCircle(100, 100, r, painoutline);
            }
            else if (section.LengthX != null)
            {
                canvas.DrawCircle(100, 100, 100, paint);
            }
            return TransformSVG(svgpath, canvas, info, xmin, ymin, xmax, ymax, wx, wy);
        }

        public static SKCanvas TransformSVG(string svgpath, SKCanvas canvas, SKImageInfo info, double xmin, double ymin, double xmax, double ymax, double wx, double wy)
        {
            SKPath catPath = SKPath.ParseSvgPathData(svgpath);
            SKPaint paint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                Color = SKColors.Black,
            };
            if (svgpath != null && xmax > xmin && ymax > ymin)
            {
                double mx = xmax - xmin;
                double my = ymax - ymin;

                double scalex = wx / mx;
                double scaley = wy / my;
                float scale = (float)Math.Min(scalex, scaley);

                double mm = 0.5 * Math.Max(mx, my);
                double midx = 0.5 * (xmin + xmax);
                double midy = 0.5 * (ymin + ymax);

                float offsetx =(float) (mm - midx);
                float offsety =(float) (-mm - midy);
                
                canvas.Translate(offsetx, offsety);
                canvas.Scale(info.Width, info.Width);
                canvas.DrawPath(catPath, paint);

                return canvas;
            }
            return null;
        }
        private static string LayoutSVG(
       Contour contour,
       out double xmin, out double ymin, out double xmax, out double ymax)
        {
            xmin = 0.0;
            xmax = 0.0;
            ymin = 0.0;
            ymax = 0.0;

            if (contour.Segments == null)
                return null;

            double offsetx = contour.X;
            double offsety = contour.Y;

            Segment[] listSegments = contour.Segments;

            return LayoutSVG(offsetx, offsety, contour.Segments, out xmin, out ymin, out xmax, out ymax);
        }
        private static string LayoutSVG(
        double offsetx,
        double offsety,
        Segment[] listSegments,
        out double xmin, out double ymin, out double xmax, out double ymax)
        {
            xmin = 0.0;
            xmax = 0.0;
            ymin = 0.0;
            ymax = 0.0;

            if (listSegments == null)
                return null;

            StringBuilder sbPath = new StringBuilder();

            sbPath.Append("M" + offsetx + " " + offsety + " ");

            double scale = 1.0;

            double x = offsetx;
            double y = offsety;

            for (int i = 0; i < listSegments.Length; i++)
            {
                Segment segment = listSegments[i];

                double length = segment.Length.GetValueOrDefault() * scale;
                double angle = segment.Angle.GetValueOrDefault();
                double radius = segment.Radius.GetValueOrDefault() * scale;

                double prevangle = listSegments[listSegments.Length - 1].Angle.GetValueOrDefault();
                double nextangle = listSegments[0].Angle.GetValueOrDefault();
                if (i < listSegments.Length - 1)
                {
                    nextangle = listSegments[i + 1].Angle.GetValueOrDefault();
                }
                if (i > 0)
                {
                    prevangle = listSegments[i - 1].Angle.GetValueOrDefault();
                }

                if (segment.Angle == null)
                {
                    angle = prevangle;
                }

                double dx = 0.0;
                double dy = 0.0;

                if (radius != 0.0)
                {
                    int sweep = (radius > 0.0) ? 1 : 0;

                    double cx, cy; // circle center point
                    double anglediff;
                    if (sweep > 0)
                    {
                        // counter-clockwise
                        cx = radius * Math.Cos((prevangle + 90.0) * Math.PI / 180.0);
                        cy = radius * Math.Sin((prevangle + 90.0) * Math.PI / 180.0);
                        dx = cx + radius * Math.Sin(nextangle * Math.PI / 180.0);
                        dy = cy - radius * Math.Cos(nextangle * Math.PI / 180.0);

                        anglediff = nextangle - prevangle;
                    }
                    else
                    {
                        // clockwise
                        radius = -radius;
                        cx = radius * Math.Cos((prevangle - 90.0) * Math.PI / 180.0);
                        cy = radius * Math.Sin((prevangle - 90.0) * Math.PI / 180.0);
                        dx = cx - radius * Math.Sin(nextangle * Math.PI / 180.0);
                        dy = cy + radius * Math.Cos(nextangle * Math.PI / 180.0);

                        anglediff = prevangle - nextangle;
                    }

                    // arc
                    if (anglediff < 0.0)
                        anglediff += 360.0;

                    int largearc = (anglediff > 180.0) ? 1 : 0;
                    double rotation = 0.0;

                    sbPath.Append("a " + radius + " " + radius + " " + rotation + " " + largearc + " " + sweep + " " + dx + " " + dy);
                }
                else
                {
                    // line
                    dx = length * Math.Cos(angle * Math.PI / 180.0);
                    dy = length * Math.Sin(angle * Math.PI / 180.0);
                    sbPath.Append("l" + dx + " " + dy + " ");
                }

                x += dx;
                y += dy;

                if (x < xmin)
                    xmin = x;
                if (x > xmax)
                    xmax = x;
                if (y < ymin)
                    ymin = y;
                if (y > ymax)
                    ymax = y;
            }

            return sbPath.ToString();
        }
        public static Color ConvertColor(long color)
        {
            ColorTypeConverter converter = new ColorTypeConverter();
            string _color = "#" + color.ToString();
            return (Color)converter.ConvertFromInvariantString(_color);
        }
        public static StackLayout GetTitleView(string Name, string title)
        {
            ImageSource source = DataController.GetIcon(Name + "icon");
            StackLayout titleview = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    new Label
                    {
                        Text = title,
                        FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.EndAndExpand
                    },
                    new ImageButton
                    {
                        Source = source,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        BackgroundColor = Color.Transparent,
                        WidthRequest = 50,
                        HeightRequest = 50
                    }
                }
            };
            return titleview;
        }
    }
}
