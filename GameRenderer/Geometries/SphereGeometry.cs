using System;
using System.Collections.Generic;
using GameModel;
using GlmNet;

namespace GameRenderer
{
    public class SphereGeometry : ColorVertexGeometry
    {
        private int stacks = 16;

        private int slices = 16;

        private float radius;

        private vec4 color;
        
        public SphereGeometry(float radius, vec4 color)
        {
            Update(radius, color);
        }

        public void Update(float radius, vec4 color)
        {
            this.color = color;
            this.radius = radius;
            
            var data = new List<ColorVertex>();

            for( int t = 0 ; t < stacks; t++ ) // stacks are ELEVATION so they count theta
            {
                float theta1 = ( (float)(t)/stacks )* (float)Math.PI ;
                float theta2 = ( (float)(t+1)/stacks )* (float)Math.PI ;

                for( int p = 0 ; p < slices ; p++ ) // slices are ORANGE SLICES so the count azimuth
                {
                    float phi1 = ( (float)(p)/slices )*2* (float) Math.PI ; // azimuth goes around 0 .. 2*PI
                    float phi2 = ( (float)(p+1)/slices )*2* (float) Math.PI ;

                    var vertex1 = VectorHelper.Polar2Cartesian(radius, theta1, phi1);
                    var vertex2 = VectorHelper.Polar2Cartesian(radius, theta1, phi2);
                    var vertex3 = VectorHelper.Polar2Cartesian(radius, theta2, phi2);
                    var vertex4 = VectorHelper.Polar2Cartesian(radius, theta2, phi1);

                    // facing out
                    if (t == 0)
                    {
                        // top cap
                        data.Add( new ColorVertex(vertex1, color));
                        data.Add( new ColorVertex(vertex3, color));
                        data.Add( new ColorVertex(vertex4, color));
                    } 
                    else if (t + 1 == stacks)
                    {
                        //end cap
                        data.Add( new ColorVertex(vertex3, color));
                        data.Add( new ColorVertex(vertex1, color));
                        data.Add( new ColorVertex(vertex2, color));
                    }
                    else
                    {
                        // body, facing OUT:
                        data.Add( new ColorVertex(vertex1, color));
                        data.Add( new ColorVertex(vertex2, color));
                        data.Add( new ColorVertex(vertex4, color));
                        
                        data.Add( new ColorVertex(vertex2, color));
                        data.Add( new ColorVertex(vertex3, color));
                        data.Add( new ColorVertex(vertex4, color));
                    }
                }
            }
            
            UpdateData(data.ToRawArray());
        }


    }
}