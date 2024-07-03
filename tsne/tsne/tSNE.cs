using System.Security.Policy;
using System;

namespace tsneDemo
{
    public class tSNE
    {
        public tSNE(tsneSettings opt)
        {
            this.perplexity = opt.perplexity; // effective number of nearest neighbors
            this.dim = opt.dim; // by default 2-D tSNE
            this.epsilon = opt.epsilon; // learning rate

            this.iter = 0;
        }
        int iter;
        public double epsilon = 10; // epsilon is learning rate (10 = default)
        public double perplexity = 30; // roughly how many neighbors each point influences (30 = default)
        public int dim = 2; // dimensionality of the embedding (2 = default)
        internal double[][] getSolution()
        {
            return this.Y;
        }
        // this function takes a given distance matrix and creates
        // matrix P from them.
        // D is assumed to be provided as a list of lists, and should be symmetric

        internal void initDataDist(double[][] D)
        {
            var N = D.Length;
            //assert(N > 0, " X is empty? You must have some data!");
            // convert D to a (fast) typed array version
            var dists = new double[(N * N)]; // allocate contiguous array
            for (var i = 0; i < N; i++)
            {
                for (var j = i + 1; j < N; j++)
                {
                    var d = D[i][j];
                    dists[i * N + j] = d;
                    dists[j * N + i] = d;
                }
            }
            this.P = d2p(dists, this.perplexity, 1e-4);
            this.N = N;
            this.initSolution(); // refresh this
        }

        public double[] P { get; private set; }

        int N;
        // (re)initializes the solution to random
        public void initSolution()
        {
            // generate random solution to t-SNE
            this.Y = randn2d(this.N, this.dim); // the solution
            this.gains = randn2d(this.N, this.dim, 1.0); // step gains to accelerate progress in unchanging directions
            this.ystep = randn2d(this.N, this.dim, 0.0); // momentum accumulator
            this.iter = 0;
        }

        Random r = new Random();
        public double random()
        {
            return r.NextDouble();
        }
        // Standard Normal variate using Box-Muller transform.
        double gaussRandom(double mean = 0, double stdev = 1)
        {
            double u = 1 - random(); // Converting [0,1) to (0,1]
            double v = random();
            double z = Math.Sqrt(-2.0 * Math.Log(u)) * Math.Cos(2.0 * Math.PI * v);
            // Transform to the desired mean and standard deviation:
            return z * stdev + mean;
        }
        // return random normal number
        double randn(double mu, double std) { return mu + gaussRandom() * std; }
        
        public double[][] Y { get; private set; }

        private double[][] gains;
        private double[][] ystep;

        // utility that returns 2d array filled with random numbers
        // or with value s, if provided
        double[][] randn2d(int n, int d, double? s = null)
        {
            var uses = s != null;
            List<double[]> x = new List<double[]>();
            for (var i = 0; i < n; i++)
            {
                List<double> xhere = new List<double>();
                for (var j = 0; j < d; j++)
                {
                    if (uses)
                    {
                        xhere.Add(s.Value);
                    }
                    else
                    {
                        xhere.Add(randn(0.0, 1e-4));
                    }
                }
                x.Add(xhere.ToArray());
            }
            return x.ToArray();
        }

        // return cost and gradient, given an arrangement
        Tuple<double, double[][]> costGrad(double[][] Y)
        {
            var N = this.N;
            var dim = this.dim; // dim of output space
            var P = this.P;

            var pmul = this.iter < 100 ? 4 : 1; // trick that helps with local optima

            // compute current Q distribution, unnormalized first
            var Qu = new double[(N * N)];
            var qsum = 0.0;
            for (var i = 0; i < N; i++)
            {
                for (var j = i + 1; j < N; j++)
                {
                    var dsum = 0.0;
                    for (var d = 0; d < dim; d++)
                    {
                        var dhere = Y[i][d] - Y[j][d];
                        dsum += dhere * dhere;
                    }
                    var qu = 1.0 / (1.0 + dsum); // Student t-distribution
                    Qu[i * N + j] = qu;
                    Qu[j * N + i] = qu;
                    qsum += 2 * qu;
                }
            }
            // normalize Q distribution to sum to 1
            var NN = N * N;
            var Q = new double[(NN)];
            for (var q = 0; q < NN; q++) { Q[q] = Math.Max(Qu[q] / qsum, 1e-100); }

            var cost = 0.0;
            List<double[]> grad = new List<double[]>(); ;
            for (var i = 0; i < N; i++)
            {
                var gsum = new double[(dim)]; // init grad for point i
                for (var d = 0; d < dim; d++) { gsum[d] = 0.0; }
                for (var j = 0; j < N; j++)
                {
                    cost += -P[i * N + j] * Math.Log(Q[i * N + j]); // accumulate cost (the non-constant portion at least...)
                    var premult = 4 * (pmul * P[i * N + j] - Q[i * N + j]) * Qu[i * N + j];
                    for (var d = 0; d < dim; d++)
                    {
                        gsum[d] += premult * (Y[i][d] - Y[j][d]);
                    }
                }
                grad.Add(gsum);
            }

            return new Tuple<double, double[][]>(cost,grad.ToArray());
        }

        // perform a single step of optimization to improve the embedding
        // compute L2 distance between t
        internal double step()
        {
            this.iter += 1;
            var N = this.N;

            var (cost,grad)= this.costGrad(this.Y); // evaluate gradient
            

            // perform gradient step
            var ymean = new double[(this.dim)];
            for (var i = 0; i < N; i++)
            {
                for (var d = 0; d < this.dim; d++)
                {
                    var gid = grad[i][d];
                    var sid = this.ystep[i][d];
                    var gainid = this.gains[i][d];

                    // compute gain update
                    var newgain = Math.Sign(gid) == Math.Sign(sid) ? gainid * 0.8 : gainid + 0.2;
                    if (newgain < 0.01) newgain = 0.01; // clamp
                    this.gains[i][d] = newgain; // store for next turn

                    // compute momentum step direction
                    var momval = this.iter < 250 ? 0.5 : 0.8;
                    var newsid = momval * sid - this.epsilon * newgain * grad[i][d];
                    this.ystep[i][d] = newsid; // remember the step we took

                    // step!
                    this.Y[i][d] += newsid;

                    ymean[d] += this.Y[i][d]; // accumulate mean so that we can center later
                }
            }

            // reproject Y to be zero mean
            for (var i = 0; i < N; i++)
            {
                for (var d = 0; d < this.dim; d++)
                {
                    this.Y[i][d] -= ymean[d] / N;
                }
            }

            //if(this.iter%100===0) console.log('iter ' + this.iter + ', cost: ' + cost);
            return cost; // return current cost
        }


        // compute (p_{i|j} + p_{j|i})/(2n)
        double[] d2p(double[] D, double perplexity, double tol)
        {
            var Nf = Math.Sqrt(D.Length); // this better be an integer
            var N = (int)Math.Floor(Nf);
            //assert(N === Nf, "D should have square number of elements.");
            var Htarget = Math.Log(perplexity); // target entropy of distribution
            var P = new double[(N * N)]; // temporary probability matrix

            var prow = new double[N]; // a temporary storage compartment
            for (var i = 0; i < N; i++)
            {
                var betamin = double.NegativeInfinity;
                var betamax = double.PositiveInfinity;
                double beta = 1; // initial value of precision
                var done = false;
                var maxtries = 50;

                // perform binary search to find a suitable precision beta
                // so that the entropy of the distribution is appropriate
                var num = 0;
                while (!done)
                {
                    //debugger;

                    // compute entropy and kernel row with beta precision
                    var psum = 0.0;
                    for (var j = 0; j < N; j++)
                    {
                        var pj = Math.Exp(-D[(i * N + j)] * beta);
                        if (i == j) { pj = 0; } // we dont care about diagonals
                        prow[j] = pj;
                        psum += pj;
                    }
                    // normalize p and compute entropy
                    var Hhere = 0.0;
                    for (var j = 0; j < N; j++)
                    {
                        double pj = 0;
                        if (psum == 0)
                        {
                            pj = 0;
                        }
                        else
                        {
                            pj = prow[j] / psum;
                        }
                        prow[j] = pj;
                        if (pj > 1e-7) Hhere -= pj * Math.Log(pj);
                    }

                    // adjust beta based on result
                    if (Hhere > Htarget)
                    {
                        // entropy was too high (distribution too diffuse)
                        // so we need to increase the precision for more peaky distribution
                        betamin = beta; // move up the bounds
                        if (betamax == double.PositiveInfinity) { beta = beta * 2; }
                        else { beta = (beta + betamax) / 2; }

                    }
                    else
                    {
                        // converse case. make distrubtion less peaky
                        betamax = beta;
                        if (betamin == double.NegativeInfinity) { beta = beta / 2; }
                        else { beta = (beta + betamin) / 2; }
                    }

                    // stopping conditions: too many tries or got a good precision
                    num++;
                    if (Math.Abs(Hhere - Htarget) < tol) { done = true; }
                    if (num >= maxtries) { done = true; }
                }

                // console.log('data point ' + i + ' gets precision ' + beta + ' after ' + num + ' binary search steps.');
                // copy over the final prow to P at row i
                for (var j = 0; j < N; j++) { P[i * N + j] = prow[j]; }

            } // end loop over examples i

            // symmetrize P and normalize it to sum to 1 over all ij
            var Pout = new double[(N * N)];
            var N2 = N * 2;
            for (var i = 0; i < N; i++)
            {
                for (var j = 0; j < N; j++)
                {
                    Pout[i * N + j] = Math.Max((P[i * N + j] + P[j * N + i]) / N2, 1e-100);
                }
            }

            return Pout;
        }

    }
}