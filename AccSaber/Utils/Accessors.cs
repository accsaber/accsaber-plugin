using HMUI;
using IPA.Utilities;

namespace AccSaber.Utils
{
	internal sealed class Accessors
	{
		public static readonly FieldAccessor<ModalView, bool>.Accessor ViewValidAccessor =
			FieldAccessor<ModalView, bool>.GetAccessor("_viewIsValid");
		
		public static readonly FieldAccessor<ImageView, bool>.Accessor GradientAccessor =
			FieldAccessor<ImageView, bool>.GetAccessor("_gradient");
		public static readonly FieldAccessor<ImageView, float>.Accessor SkewAccessor = 
			FieldAccessor<ImageView, float>.GetAccessor("_skew");
		public static readonly FieldAccessor<ModalView, bool>.Accessor AnimateCanvasAccessor = 
			FieldAccessor<ModalView, bool>.GetAccessor("_animateParentCanvas");
	}
}