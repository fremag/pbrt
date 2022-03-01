using NFluent;
using NUnit.Framework;
using pbrt.Spectrums;

namespace Pbrt.Tests.Spectrums
{
    [TestFixture]
    public class SampledSpectrumTests
    {
        [Test]
        public void ConstructorTest()
        {
            SampledSpectrum spec = new Spectrum(4, 1.23f);
            Check.That(spec.NSpectrumSamples).IsEqualTo(4);
            Check.That(spec.C).ContainsExactly(1.23f, 1.23f, 1.23f, 1.23f);

            Check.That(SampledSpectrum.NSpectralSamples).IsEqualTo(60);
            Check.That(SampledSpectrum.SampledLambdaStart).IsEqualTo(400);
            Check.That(SampledSpectrum.SampledLambdaEnd).IsEqualTo(700);
        } 

        [Test]
        public void ConstructorCopyTest()
        {
            SampledSpectrum spec = new SampledSpectrum(4, 1.23f);
            SampledSpectrum spec2 = new SampledSpectrum(spec);
            
            Check.That(spec2.NSpectrumSamples).IsEqualTo(4);
            Check.That(spec2.C).ContainsExactly(1.23f, 1.23f, 1.23f, 1.23f);
        }

        [Test]
        public void SpectrumSamplesSortedTest()
        {
            Check.That(SampledSpectrum.SpectrumSamplesSorted(new [] {1f, 2f, 3f}, 3)).IsTrue();
            Check.That(SampledSpectrum.SpectrumSamplesSorted(new [] {1f, 4f, 3f}, 3)).IsFalse();
        }

        [Test]
        public void SortSpectrumSamplesTest()
        {
            float[] lambdas = new [] {1.23f, 3.45f, 0.12f, 2.34f};
            float[] values = new [] {1f, 3f, 0f, 2f};
            SampledSpectrum.SortSpectrumSamples(lambdas, values, lambdas.Length, out var sortedLambdas, out var sortedValues);
            Check.That(sortedLambdas).ContainsExactly(0.12f, 1.23f, 2.34f, 3.45f);
            Check.That(sortedValues).ContainsExactly(0, 1,2,3);
        }

        [Test]
        public void LowerThanMinLambda_AverageSpectrumSamplesTest()
        {
            float[] lambdas = {100, 200, 300, 400};
            float[] values = {1f, 2f, 3f, 4f};
            var avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, lambdas.Length, 0, 50);
            Check.That(avg).IsEqualTo(1f);
        }
        
        [Test]
        public void GreaterThanMAxLambda_AverageSpectrumSamplesTest()
        {
            float[] lambdas = {100, 200, 300, 400};
            float[] values = {1f, 2f, 3f, 4f};
            var avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, lambdas.Length, 500, 600);
            Check.That(avg).IsEqualTo(4f);
        }
        
        [Test]
        public void DiracDistribution_AverageSpectrumSamplesTest()
        {
            float[] lambdas = {100};
            float[] values = {1.23f};
            var avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, 1, 0, 200);
            Check.That(avg).IsEqualTo(1.23f);
        }
        
        [Test]
        public void FullSpectrum_AverageSpectrumSamplesTest()
        {
            float[] lambdas = {100, 200, 300, 400};
            float[] values = {1f, 2f, 3f, 4f};

            var avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, lambdas.Length, 0, 500);
            Check.That(avg).IsEqualTo(2.5f);
        }

        [Test]
        public void OneBandAverageSpectrumSamplesTest()
        {
            float[] lambdas = {100, 200, 300, 400};
            float[] values = {1f, 2f, 3f, 4f};

            var avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, lambdas.Length, 200.00001f, 299.99999f);
            var expected = 0.5f*(3+2)*(300-200)/(300-200);
            Check.That(avg).IsEqualTo(expected);
        }

        [Test]
        public void InterpAverageSpectrumSamplesTest()
        {
            float[] lambdas = {100, 200, 300, 400};
            float[] values = {1f, 2f, 3f, 4f};

            var avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, lambdas.Length, 150, 350f);
            Check.That(avg).IsEqualTo(2.5);
        }

        [Test]
        public void ConstantSpectrum_AverageSpectrumSamplesTest()
        {
            float[] lambdas = {100, 200, 300, 400};
            float[] values = {1f, 1f, 1f, 1f};

            var avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, lambdas.Length, 150, 350f);
            Check.That(avg).IsEqualTo(1);

            avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, lambdas.Length, 50, 500f);
            Check.That(avg).IsEqualTo(1);
        }

        [Test]
        public void FromSampledTest()
        {
            float[] lambdas = new [] {500f, 400f, 600f, 700f};
            float[] values = new [] {2f, 1f, 2f, 1f};

            var spec = SampledSpectrum.FromSampled(lambdas, values, lambdas.Length);

            float[] expected =
            {
                1.025f, 1.075f, 1.125f, 1.175f, 1.225f, 1.275f, 1.325f, 1.375f,
                1.425f, 1.475f, 1.525f, 1.575f, 1.625f, 1.675f, 1.725f, 1.775f, 1.825f, 1.875f, 1.925f, 1.975f,
                2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f,
                1.975f, 1.925f, 1.875f, 1.825f, 1.775f, 1.725f, 1.675f, 1.625f, 1.575f, 1.525f, 1.475f,
                1.425f, 1.375f, 1.325f, 1.275f, 1.225f, 1.175f, 1.125f, 1.075f, 1.025f
            };
            for (int i = 0; i < spec.C.Length; i++)
            {
                Check.That(spec.C[i]).IsCloseTo(expected[i], 1e-3);
            }
        }

        [Test]
        public void ToXyzTest()
        {
            float[] lambdas = new [] {500f, 400f, 600f, 700f};
            float[] values = new [] {2f, 1f, 2f, 1f};

            var spec = SampledSpectrum.FromSampled(lambdas, values, lambdas.Length);
            var xyz = spec.ToXYZ();
            Check.That(xyz[0]).IsCloseTo(1.81246662, 1e-5);
            Check.That(xyz[1]).IsCloseTo(1.94576085, 1e-5);
            Check.That(xyz[2]).IsCloseTo(1.53008139, 1e-5);
        }

        [Test]
        public void YTest()
        {
            float[] lambdas = new [] {500f, 400f, 600f, 700f};
            float[] values = new [] {2f, 1f, 2f, 1f};

            var spec = SampledSpectrum.FromSampled(lambdas, values, lambdas.Length);
            var y = spec.Y();
            Check.That(y).IsCloseTo(207.91795, 1e-4);
        }

        [Test]
        public void ToRgbTest()
        {
            float[] lambdas = new [] {500f, 400f, 600f, 700f};
            float[] values = new [] {2f, 1f, 2f, 1f};

            var spec = SampledSpectrum.FromSampled(lambdas, values, lambdas.Length);
            var rgb = spec.ToRgb();
            Check.That(rgb[0]).IsCloseTo(2.11953449, 1e-4);
            Check.That(rgb[1]).IsCloseTo(1.95706964, 1e-4);
            Check.That(rgb[2]).IsCloseTo(1.32161331, 1e-4);
        }

        [Test]
        public void RgbConstructorTest()
        {
            RgbSpectrum rgbSpectrum = new RgbSpectrum(new []{1f, 2f, 3f});
            var spec = new SampledSpectrum(rgbSpectrum, SpectrumType.Illuminant);
            Check.That(spec.C).ContainsExactly(2.89233756f, 2.8918798f, 2.89135337f, 2.89301944f, 2.89513659f, 2.89602733f, 2.89637351f, 2.89666295f, 2.89690208f, 2.8970418f, 2.89700103f, 2.8967061f, 2.89553428f, 2.89450264f, 2.89505005f, 2.89568806f, 2.89160705f, 2.88507223f, 2.72362041f, 2.44119644f, 2.21684027f, 2.08623648f, 1.98586059f, 1.97615242f, 1.9727186f, 1.96741915f, 1.96195006f, 1.95918274f, 1.95738494f, 1.95632577f, 1.95577955f, 1.9395057f, 1.90058553f, 1.85024047f, 1.76837277f, 1.67759693f, 1.54726875f, 1.41060328f, 1.27168679f, 1.13245904f, 1.02317572f, 0.932436466f, 0.863171697f, 0.821402311f, 0.791733205f, 0.792201281f, 0.797062755f, 0.816552341f, 0.837735713f, 0.855436563f, 0.87201345f, 0.882253468f, 0.88898617f, 0.890395284f, 0.885767221f, 0.883981645f, 0.8886078f, 0.893407583f, 0.897905946f, 0.901720047f);
        }

        [Test]
        public void ToRgbSpectrumTest()
        {
            float[] lambdas = new [] {500f, 400f, 600f, 700f};
            float[] values = new [] {2f, 1f, 2f, 1f};

            var spec = SampledSpectrum.FromSampled(lambdas, values, lambdas.Length);
            var rgbSpectrum = spec.ToRgbSpectrum();
            Check.That(rgbSpectrum.NSpectrumSamples).IsEqualTo(3);
            Check.That(rgbSpectrum.C[0]).IsCloseTo(2.119f, 1e-3);
            Check.That(rgbSpectrum.C[1]).IsCloseTo(1.957f, 1e-3);
            Check.That(rgbSpectrum.C[2]).IsCloseTo(1.321f, 1e-3);
        }

        [Test]
        public void FromXyzTest()
        {
            var xyz = new float[] { 1.23f, 2.34f, 3.45f };
            var spec = SampledSpectrum.FromXYZ(xyz);
            Check.That(spec.C).ContainsExactly(3.0316808f, 3.0690072f, 3.1040704f, 3.0670853f, 3.0168383f, 3.0480032f, 3.1152909f, 3.148962f, 3.1530006f, 3.1619635f, 3.1798198f, 3.1803367f, 3.1194837f, 3.066788f, 3.1127589f, 3.1774743f, 3.219369f, 3.252134f, 3.2727885f, 3.2839832f, 3.2913399f, 3.2924967f, 3.293465f, 3.2941072f, 3.2944043f, 3.2926345f, 3.2906017f, 3.2903123f, 3.2906604f, 3.2929385f, 3.2965567f, 3.2989712f, 3.2996504f, 3.2948048f, 3.2745092f, 3.1441374f, 2.3994243f, 1.556535f, 0.682367f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
        }
/*
 *                     if (rgb[0] <= rgb[1] && rgb[0] <= rgb[2])
                        if (rgb[1] <= rgb[2])
                        else
                    else if (rgb[1] <= rgb[0] && rgb[1] <= rgb[2])
                        if (rgb[0] <= rgb[2])
                        else
                    else
                        if (rgb[0] <= rgb[1])
                        else
 */
        [Test]
        public void FromRgb_Reflectance_CyanBlue_Test()
        {
            var rgb = new float[] { 1, 2, 3 };
            var spec = SampledSpectrum.FromRgb(rgb, SpectrumType.Reflectance);
            Check.That(spec.C).ContainsExactly(2.8875768f,  2.896323f,  2.904222f,  2.8949487f,  2.8825881f,  2.8915792f,  2.9100428f,  2.919f,  2.9195766f,  2.921388f,  2.9254296f,  2.924184f,  2.904262f,  2.877555f,  2.8129253f,  2.739919f,  2.6513367f,  2.556547f,  2.459766f,  2.3614302f,  2.2686963f,  2.185007f,  2.1090846f,  2.0570982f,  2.0117276f,  1.9976221f,  1.9894412f,  1.9874575f,  1.9876881f,  1.9884228f,  1.9895072f,  1.9905626f,  1.9915742f,  1.9914901f,  1.9883349f,  1.9611003f,  1.7993762f,  1.6162949f,  1.4282361f,  1.2395242f,  1.119638f,  1.0423734f,  1.0037339f,  1.0145489f,  1.0230944f,  1.0254797f,  1.0283264f,  1.0349406f,  1.0417032f,  1.0442374f,  1.045606f,  1.0490013f,  1.0535184f,  1.0501957f,  1.0379822f,  1.0310682f,  1.0361029f,  1.0391328f,  1.0313354f,  1.0225058f);
        }

        [Test]
        public void FromRgb_Reflectance_CyanGreen_Test()
        {
            var rgb = new float[] { 1, 3, 2 };
            var spec = SampledSpectrum.FromRgb(rgb, SpectrumType.Reflectance);
            Check.That(spec.C).ContainsExactly(1.9408267f,  1.9503824f,  1.9595432f,  1.9506457f,  1.9384139f,  1.9463062f,  1.9631275f,  1.971428f,  1.9722166f,  1.9739308f,  1.9773167f,  1.9835399f,  1.9995811f,  2.0303226f,  2.1626205f,  2.3154657f,  2.4708657f,  2.627145f,  2.7465243f,  2.8370893f,  2.8988602f,  2.9141405f,  2.9256368f,  2.927263f,  2.927989f,  2.927526f,  2.9269073f,  2.9272187f,  2.9278626f,  2.928626f,  2.9294715f,  2.929958f,  2.9299254f,  2.924636f,  2.9046197f,  2.843673f,  2.5573084f,  2.2342665f,  1.8907238f,  1.5419937f,  1.2931075f,  1.1060519f,  0.999912f,  0.9973747f,  0.99475944f,  0.9909835f,  0.9879176f,  0.9886592f,  0.98986346f,  0.9904138f,  0.99083275f,  0.99395174f,  0.9985652f,  0.9973402f,  0.9894938f,  0.9869035f,  0.996159f,  1.0039319f,  1.0031959f,  1.0017856f);
        }
        
        [Test]
        public void FromRgb_Reflectance_MagentaBlue_Test()
        {
            var rgb = new float[] { 2, 1, 3 };
            var spec = SampledSpectrum.FromRgb(rgb, SpectrumType.Reflectance);
            Check.That(spec.C).ContainsExactly(2.8585203f,  2.864091f,  2.8703513f,  2.8785198f,  2.886961f,  2.8911865f,  2.893539f,  2.895727f,  2.8977692f,  2.8952796f,  2.884605f,  2.875881f,  2.874577f,  2.865983f,  2.799238f,  2.7146907f,  2.5033145f,  2.2412255f,  1.9174256f,  1.545437f,  1.2835507f,  1.1994301f,  1.1245406f,  1.0709904f,  1.0239843f,  1.0089898f,  0.9999148f,  0.99472874f,  0.9909732f,  0.9927153f,  0.99827486f,  1.000796f,  0.99894136f,  1.0220529f,  1.115253f,  1.2227789f,  1.3873892f,  1.5595516f,  1.7129405f,  1.8591248f,  1.9163535f,  1.9184943f,  1.9316006f,  1.9587468f,  1.9777033f,  1.9759605f,  1.9710268f,  1.955184f,  1.9364042f,  1.9008421f,  1.8615471f,  1.8738384f,  1.9146829f,  1.9337419f,  1.9280945f,  1.9292212f,  1.94565f,  1.9495302f,  1.8955371f,  1.8397152f);
        }
        
        [Test]
        public void FromRgb_Reflectance_MagentaRed_Test()
        {
            var rgb = new float[] { 3, 1, 2 };
            var spec = SampledSpectrum.FromRgb(rgb, SpectrumType.Reflectance);
            Check.That(spec.C).ContainsExactly(2.0386968f,  2.0401626f,  2.0412307f,  2.037526f,  2.0323172f,  2.0159552f,  1.9946427f,  1.9760795f,  1.959939f,  1.9499807f,  1.9511887f,  1.9523784f,  1.9527029f,  1.9538016f,  1.9611253f,  1.9636906f,  1.8414447f,  1.6693101f,  1.441164f,  1.1692797f,  0.9997094f,  0.99533045f,  0.99410826f,  0.9957388f,  0.99682677f,  0.99311465f,  0.9886399f,  0.98566985f,  0.98328507f,  0.98704356f,  0.9950675f,  1.0002923f,  1.0014864f,  1.0250217f,  1.1112798f,  1.2478135f,  1.6462379f,  2.078833f,  2.418708f,  2.726404f,  2.8448641f,  2.846135f,  2.857216f,  2.8808572f,  2.8944438f,  2.8826573f,  2.868716f,  2.8501327f,  2.8298311f,  2.7969615f,  2.7613714f,  2.7680595f,  2.7981486f,  2.8184927f,  2.8277848f,  2.8362155f,  2.8427305f,  2.8392868f,  2.7922525f,  2.7432673f);
        }
        
        [Test]
        public void FromRgb_Reflectance_YellowGreen_Test()
        {
            var rgb = new float[] { 2, 3, 1 };
            var spec = SampledSpectrum.FromRgb(rgb, SpectrumType.Reflectance);
            Check.That(spec.C).ContainsExactly(0.9814926f,  0.98231465f,  0.9832801f,  0.9825329f,  0.9815212f,  0.98379076f,  0.9875199f,  0.993543f,  1.0015885f,  1.0185648f,  1.0516737f,  1.0951816f,  1.1739572f,  1.2683983f,  1.4553605f,  1.6617088f,  1.8842218f,  2.113063f,  2.3132777f,  2.4911387f,  2.6360786f,  2.7278652f,  2.810392f,  2.8661826f,  2.9139764f,  2.92295f,  2.924958f,  2.9257958f,  2.926214f,  2.9263391f,  2.926261f,  2.9262657f,  2.9263885f,  2.922277f,  2.9062905f,  2.873048f,  2.74667f,  2.6047478f,  2.4489577f,  2.2891893f,  2.1624584f,  2.0561867f,  1.9940276f,  1.9883502f,  1.9842229f,  1.9834635f,  1.9827728f,  1.9816169f,  1.9804306f,  1.9796292f,  1.9789377f,  1.9782834f,  1.9776496f,  1.9769965f,  1.9763222f,  1.9755816f,  1.9746926f,  1.9746505f,  1.9783679f,  1.9822041f);
        }
        
        [Test]
        public void FromRgb_Reflectance_YellowRed_Test()
        {
            var rgb = new float[] { 3, 2, 1 };
            var spec = SampledSpectrum.FromRgb(rgb, SpectrumType.Reflectance);
            Check.That(spec.C).ContainsExactly(1.1084193f,  1.1043268f,  1.0988382f,  1.085842f,  1.0710517f,  1.0538325f,  1.035539f,  1.0214676f,  1.0111182f,  1.020723f,  1.0663701f,  1.112323f,  1.1567638f,  1.2034495f,  1.267553f,  1.3351618f,  1.4028232f,  1.4705496f,  1.5502578f,  1.6393223f,  1.7220737f,  1.7946322f,  1.863407f,  1.9207665f,  1.9705576f,  1.977171f,  1.9762168f,  1.9769757f,  1.9783512f,  1.9804642f,  1.9830891f,  1.9863665f,  1.9905821f,  1.9921f,  1.9860326f,  2.01551f,  2.2475867f,  2.5060575f,  2.6922376f,  2.8539991f,  2.9174993f,  2.9201486f,  2.9234648f,  2.9276347f,  2.929298f,  2.9246564f,  2.9208708f,  2.922847f,  2.9256976f,  2.929572f,  2.9335353f,  2.927554f,  2.9160686f,  2.9146028f,  2.9245007f,  2.9267406f,  2.9117172f,  2.899608f,  2.9032228f,  2.9064765f);
        }
        [Test]
        public void FromRgb_Illuminant_CyanBlue_Test()
        {
            var rgb = new float[] { 1, 2, 3 };
            var spec = SampledSpectrum.FromRgb(rgb, SpectrumType.Illuminant);
            Check.That(spec.C).ContainsExactly(2.8923376f,  2.8918798f,  2.8913534f,  2.8930194f,  2.8951366f,  2.8960273f,  2.8963735f,  2.896663f,  2.896902f,  2.8970418f,  2.897001f,  2.896706f,  2.8955343f,  2.8945026f,  2.89505f,  2.895688f,  2.891607f,  2.8850722f,  2.7236204f,  2.4411964f,  2.2168403f,  2.0862365f,  1.9858606f,  1.9761524f,  1.9727186f,  1.9674191f,  1.9619501f,  1.9591827f,  1.957385f,  1.9563258f,  1.9557796f,  1.9395057f,  1.9005855f,  1.8502405f,  1.7683728f,  1.6775969f,  1.5472687f,  1.4106033f,  1.2716868f,  1.132459f,  1.0231757f,  0.93243647f,  0.8631717f,  0.8214023f,  0.7917332f,  0.7922013f,  0.79706275f,  0.81655234f,  0.8377357f,  0.85543656f,  0.87201345f,  0.88225347f,  0.8889862f,  0.8903953f,  0.8857672f,  0.88398165f,  0.8886078f,  0.8934076f,  0.89790595f,  0.90172005f);
        }

        [Test]
        public void FromRgb_Illuminant_CyanGreen_Test()
        {
            var rgb = new float[] { 1, 3, 2 };
            var spec = SampledSpectrum.FromRgb(rgb, SpectrumType.Illuminant);
            Check.That(spec.C).ContainsExactly(1.9868971f,  1.9864855f,  1.9865962f,  1.9845743f,  1.9821409f,  1.9816138f,  1.9819285f,  1.9775755f,  1.9691086f,  1.9688206f,  1.9833052f,  1.9938462f,  1.9895144f,  1.9974674f,  2.1049037f,  2.2371714f,  2.4696074f,  2.7413616f,  2.8641388f,  2.8705828f,  2.8744326f,  2.8740928f,  2.873345f,  2.871495f,  2.8681817f,  2.856006f,  2.8430672f,  2.8437233f,  2.849207f,  2.8520517f,  2.8530648f,  2.8370411f,  2.7964869f,  2.7438874f,  2.6580684f,  2.560935f,  2.411278f,  2.234398f,  1.7948571f,  1.2750747f,  1.0216537f,  0.93318343f,  0.86416084f,  0.8200385f,  0.78573954f,  0.77588284f,  0.7695678f,  0.77501017f,  0.7810161f,  0.7749882f,  0.7656318f,  0.7614807f,  0.7602109f,  0.75833875f,  0.75578386f,  0.7526084f,  0.7480377f,  0.745809f,  0.754139f,  0.7639971f);
        }
        
        [Test]
        public void FromRgb_Illuminant_MagentaBlue_Test()
        {
            var rgb = new float[] { 2, 1, 3 };
            var spec = SampledSpectrum.FromRgb(rgb, SpectrumType.Illuminant);
            Check.That(spec.C).ContainsExactly(2.841832f,  2.8411167f,  2.8401625f,  2.842829f,  2.8461802f,  2.8459105f,  2.8440337f,  2.8429122f,  2.842456f,  2.8421834f,  2.8422418f,  2.8428624f,  2.8454282f,  2.848003f,  2.8494115f,  2.8486736f,  2.8082294f,  2.7511787f,  2.4781601f,  2.0364962f,  1.6434637f,  1.3289487f,  1.079015f,  1.0279777f,  0.9948971f,  0.9845795f,  0.97851676f,  0.97598726f,  0.9747242f,  0.9742442f,  0.9743084f,  0.9621842f,  0.9325102f,  0.9109342f,  0.91245264f,  0.9250623f,  0.99233806f,  1.0689639f,  1.1568502f,  1.2487183f,  1.3689789f,  1.5068227f,  1.6121557f,  1.6758627f,  1.724637f,  1.7362914f,  1.7463666f,  1.7574767f,  1.7680107f,  1.770246f,  1.7701979f,  1.7742838f,  1.7806585f,  1.7920352f,  1.8090848f,  1.8114424f,  1.7807139f,  1.7604266f,  1.7916818f,  1.8256713f);
        }
        
        [Test]
        public void FromRgb_Illuminant_MagentaRed_Test()
        {
            var rgb = new float[] { 3, 1, 2 };
            var spec = SampledSpectrum.FromRgb(rgb, SpectrumType.Illuminant);
            Check.That(spec.C).ContainsExactly(1.9820123f,  1.9804969f,  1.9781189f,  1.9756984f,  1.9732025f,  1.9689494f,  1.9639156f,  1.9583707f,  1.9523743f,  1.9457126f,  1.937848f,  1.9319502f,  1.933027f,  1.9346777f,  1.93612f,  1.9357843f,  1.8993922f,  1.848422f,  1.7364546f,  1.5768555f,  1.407975f,  1.2241086f,  1.0738076f,  1.0292553f,  0.99595064f,  0.9856388f,  0.9796174f,  0.9770423f,  0.97570336f,  0.9752685f,  0.9754617f,  0.9673637f,  0.9473277f,  0.94652224f,  0.9998961f,  1.0690211f,  1.2085978f,  1.3606787f,  1.533541f,  1.7133148f,  1.9133893f,  2.126036f,  2.2934377f,  2.4029093f,  2.4903975f,  2.5232055f,  2.548601f,  2.5502295f,  2.5477226f,  2.5348659f,  2.5191925f,  2.5102184f,  2.5049524f,  2.513064f,  2.5363464f,  2.538533f,  2.493198f,  2.4611957f,  2.495891f,  2.535004f);
        }
        [Test]
        public void FromRgb_Illuminant_YellowGreen_Test()
        {
            var rgb = new float[] { 2, 3, 1 };
            var spec = SampledSpectrum.FromRgb(rgb, SpectrumType.Illuminant);
            Check.That(spec.C).ContainsExactly(1.0062118f,  1.0052421f,  1.0051376f,  1.0029097f,  1.0002747f,  0.9996117f,  0.99982005f,  0.9953552f,  0.986771f,  0.9863935f,  1.0008386f,  1.0155107f,  1.0299137f,  1.073288f,  1.3292394f,  1.6342137f,  2.0693877f,  2.5553901f,  2.7729454f,  2.7808762f,  2.786258f,  2.787523f,  2.7879033f,  2.785687f,  2.781992f,  2.770033f,  2.7574117f,  2.758293f,  2.7639682f,  2.7669218f,  2.7679853f,  2.7557971f,  2.7245262f,  2.6992953f,  2.6914237f,  2.6799138f,  2.6420982f,  2.5815964f,  2.2645664f,  1.8688662f,  1.7111282f,  1.700754f,  1.6889307f,  1.6752523f,  1.6454146f,  1.5750049f,  1.5083271f,  1.4699085f,  1.434983f,  1.3958876f,  1.3558009f,  1.3254393f,  1.3004605f,  1.284715f,  1.2794414f,  1.2743897f,  1.2698147f,  1.2654513f,  1.2619413f,  1.2596215f);
        }
        
        [Test]
        public void FromRgb_Illuminant_YellowRed_Test()
        {
            var rgb = new float[] { 3, 2, 1 };
            var spec = SampledSpectrum.FromRgb(rgb, SpectrumType.Illuminant);
            Check.That(spec.C).ContainsExactly(1.0518327f,  1.0500168f,  1.047851f,  1.0442241f,  1.0402925f,  1.0370642f,  1.034147f,  1.0299006f,  1.0244827f,  1.0181438f,  1.0101407f,  1.0074587f,  1.0235325f,  1.0569977f,  1.2060941f,  1.379841f,  1.5825499f,  1.796344f,  1.8907216f,  1.8918492f,  1.8931768f,  1.8948267f,  1.8952117f,  1.891622f,  1.8875824f,  1.8825055f,  1.877395f,  1.8748072f,  1.8731257f,  1.87222f,  1.8718536f,  1.8634412f,  1.8434422f,  1.8412365f,  1.8891717f,  1.9405345f,  1.9943489f,  2.0495164f,  2.1180868f,  2.1908472f,  2.2570608f,  2.3192205f,  2.3692238f,  2.403663f,  2.4171684f,  2.378238f,  2.3380563f,  2.3042035f,  2.2714148f,  2.2409558f,  2.211177f,  2.1821463f,  2.1535296f,  2.1378002f,  2.1366863f,  2.1328537f,  2.1228688f,  2.1138191f,  2.1099174f,  2.1066768f);
        }
    }
}