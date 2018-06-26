﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GLSLLanguageIntegration.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GLSLLanguageIntegration.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #
        ///#define
        ///#undef
        ///#if
        ///#ifdef
        ///#ifndef
        ///#else
        ///#elif
        ///#endif
        ///#error
        ///#pragma
        ///#extension
        ///#version
        ///#line
        ///defined
        ///##.
        /// </summary>
        internal static string Directives {
            get {
                return ResourceManager.GetString("Directives", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to radians degrees 
        ///sin cos tan 
        ///asin acos atan
        ///sinh cosh tanh 
        ///asinh acosh atanh 
        ///pow exp log exp2 log2 
        ///sqrt inversesqrt
        ///abs sign floor trunc round roundEven ceil fract mod modf
        ///min max clamp mix step smoothstep
        ///isnan isinf
        ///floatBitsToInt floatBitsToUint 
        ///intBitsToFloat uintBitsToFloat
        ///fma frexp ldexp
        ///packUnorm2x16 packSnorm2x16 packUnorm4x8 packSnorm4x8 
        ///unpackUnorm2x16 unpackSnorm2x16 unpackUnorm4x8 unpackSnorm4x8 
        ///packDouble2x32
        ///unpackDouble2x32
        ///packHalf2x16
        ///unpackHalf2x16
        ///length distan [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Functions {
            get {
                return ResourceManager.GetString("Functions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to attribute const uniform varying buffer shared
        ///coherent volatile restrict readonly writeonly
        ///atomic_uint
        ///layout
        ///centroid flat smooth noperspective
        ///patch sample
        ///break continue do for while switch case default
        ///if else
        ///subroutine
        ///in out inout
        ///float double int void bool true false
        ///invariant precise
        ///discard return
        ///struct
        ///
        ///common partition active
        ///asm
        ///class union enum typedef template this
        ///resource
        ///20
        ///3 Basics
        ///goto
        ///inline noinline public static extern external interface
        ///long short half fixed  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Keywords {
            get {
                return ResourceManager.GetString("Keywords", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to void
        ///for functions that do not return a value
        ///
        ///bool
        ///a conditional type, taking on values of true or false
        ///
        ///int
        ///a signed integer
        ///
        ///uint
        ///an unsigned integer
        ///
        ///float
        ///a single-precision floating-point scalar
        ///
        ///double
        ///a double-precision floating-point scalar
        ///
        ///vec2
        ///a two-component single-precision floating-point vector
        ///
        ///vec3
        ///a three-component single-precision floating-point vector
        ///
        ///vec4
        ///a four-component single-precision floating-point vector
        ///
        ///dvec2
        ///a two-component double-precision floatin [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Types {
            get {
                return ResourceManager.GetString("Types", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to gl_NumWorkGroups
        ///gl_WorkGroupSize
        ///gl_WorkGroupID
        ///gl_LocalInvocationID
        ///gl_GlobalInvocationID
        ///gl_LocalInvocationIndex
        ///
        ///gl_VertexID
        ///gl_VertexIndex
        ///gl_InstanceID
        ///gl_InstanceIndex
        ///gl_PerVertex
        ///gl_Position
        ///gl_PointSize
        ///gl_ClipDistance
        ///gl_CullDistance
        ///gl_in
        ///
        ///gl_PrimitiveIDIn
        ///gl_InvocationID
        ///
        ///gl_PrimitiveID
        ///gl_Layer
        ///gl_ViewportIndex
        ///
        ///gl_PatchVerticesIn
        ///gl_PrimitiveID
        ///gl_InvocationID
        ///
        ///gl_TessLevelOuter
        ///gl_TessLevelInner
        ///
        ///gl_PatchVerticesIn
        ///gl_PrimitiveID
        ///gl_TessCoord
        ///
        ///gl_FragColor [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Variables {
            get {
                return ResourceManager.GetString("Variables", resourceCulture);
            }
        }
    }
}