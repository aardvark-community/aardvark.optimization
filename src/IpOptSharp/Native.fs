/// <summary>
/// Native interop declarations for the IPOPT (Interior Point OPTimizer) C library.
/// This namespace contains P/Invoke declarations and callback delegate types for interfacing with IPOPT.
/// </summary>
namespace IpOptSharp.Native

open System
open System.Runtime.InteropServices
open System.Security

/// <summary>
/// Callback delegate for evaluating the objective function value.
/// </summary>
/// <param name="n">Number of variables</param>
/// <param name="x">Pointer to the variable values array</param>
/// <param name="new_x">Flag indicating if the variable values have changed since the last call</param>
/// <param name="obj_value">Pointer to store the computed objective value</param>
/// <returns>True if evaluation succeeded, false otherwise</returns>
[<UnmanagedFunctionPointer(CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
type EvalF = delegate of int * nativeint * bool * nativeint -> bool

/// <summary>
/// Callback delegate for evaluating the gradient of the objective function.
/// </summary>
/// <param name="n">Number of variables</param>
/// <param name="x">Pointer to the variable values array</param>
/// <param name="new_x">Flag indicating if the variable values have changed since the last call</param>
/// <param name="grad_f">Pointer to store the gradient values</param>
/// <returns>True if evaluation succeeded, false otherwise</returns>
[<UnmanagedFunctionPointer(CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
type EvalGradF = delegate of int * nativeint * bool * nativeint -> bool

/// <summary>
/// Callback delegate for evaluating the constraint function values.
/// </summary>
/// <param name="n">Number of variables</param>
/// <param name="x">Pointer to the variable values array</param>
/// <param name="new_x">Flag indicating if the variable values have changed since the last call</param>
/// <param name="m">Number of constraints</param>
/// <param name="g">Pointer to store the constraint values</param>
/// <returns>True if evaluation succeeded, false otherwise</returns>
[<UnmanagedFunctionPointer(CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
type EvalG = delegate of int * nativeint * bool * int * nativeint -> bool

/// <summary>
/// Callback delegate for evaluating the Jacobian of the constraints.
/// </summary>
/// <param name="n">Number of variables</param>
/// <param name="x">Pointer to the variable values array</param>
/// <param name="new_x">Flag indicating if the variable values have changed since the last call</param>
/// <param name="m">Number of constraints</param>
/// <param name="nele_jac">Number of non-zero elements in the Jacobian</param>
/// <param name="iRow">Pointer to row indices (or null if values is not null)</param>
/// <param name="jCol">Pointer to column indices (or null if values is not null)</param>
/// <param name="values">Pointer to Jacobian values (or null to return structure)</param>
/// <returns>True if evaluation succeeded, false otherwise</returns>
[<UnmanagedFunctionPointer(CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
type EvalJacG = delegate of int * nativeint * bool * int * int * nativeint * nativeint * nativeint -> bool

/// <summary>
/// Callback delegate for evaluating the Hessian of the Lagrangian.
/// </summary>
/// <param name="n">Number of variables</param>
/// <param name="x">Pointer to the variable values array</param>
/// <param name="new_x">Flag indicating if the variable values have changed since the last call</param>
/// <param name="obj_factor">Factor for the objective function in the Lagrangian</param>
/// <param name="m">Number of constraints</param>
/// <param name="lambda">Pointer to the constraint multipliers</param>
/// <param name="new_lambda">Flag indicating if the multipliers have changed</param>
/// <param name="nele_hess">Number of non-zero elements in the Hessian</param>
/// <param name="iRow">Pointer to row indices (or null if values is not null)</param>
/// <param name="jCol">Pointer to column indices (or null if values is not null)</param>
/// <param name="values">Pointer to Hessian values (or null to return structure)</param>
/// <returns>True if evaluation succeeded, false otherwise</returns>
[<UnmanagedFunctionPointer(CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
type EvalH = delegate of int * nativeint * bool * float * int * nativeint * bool * int * nativeint * nativeint * nativeint -> bool

/// <summary>
/// Callback invoked by IPOPT once per iteration (also during the restoration phase).
/// </summary>
/// <param name="alg_mod">Algorithm mode: 0 = regular, 1 = restoration phase</param>
/// <param name="iter_count">Current iteration count</param>
/// <param name="obj_value">Unscaled objective value at the current point</param>
/// <param name="inf_pr">Scaled primal infeasibility</param>
/// <param name="inf_du">Scaled dual infeasibility</param>
/// <param name="mu">Barrier parameter</param>
/// <param name="d_norm">Infinity norm of the primal step</param>
/// <param name="regularization_size">Regularization term of the Hessian</param>
/// <param name="alpha_du">Dual step size</param>
/// <param name="alpha_pr">Primal step size</param>
/// <param name="ls_trials">Number of line search trials</param>
/// <param name="user_data">Pointer to user data (unused)</param>
/// <returns>True to continue, false to stop the solver (status UserRequestedStop)</returns>
[<UnmanagedFunctionPointer(CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
type Intermediate = delegate of int * int * float * float * float * float * float * float * float * float * int * nativeint -> [<MarshalAs(UnmanagedType.U1)>] bool

/// <summary>
/// Module containing P/Invoke declarations for the IPOPT C library functions.
/// </summary>
module IpoptNative =

    /// <summary>
    /// Creates a new IPOPT problem instance.
    /// </summary>
    /// <param name="n">Number of optimization variables</param>
    /// <param name="x_L">Pointer to lower bounds on variables</param>
    /// <param name="x_U">Pointer to upper bounds on variables</param>
    /// <param name="m">Number of constraints</param>
    /// <param name="g_L">Pointer to lower bounds on constraints</param>
    /// <param name="g_U">Pointer to upper bounds on constraints</param>
    /// <param name="nele_jac">Number of non-zero elements in the constraint Jacobian</param>
    /// <param name="nele_hess">Number of non-zero elements in the Hessian (0 for approximation)</param>
    /// <param name="index_style">Indexing style (0 for C-style, 1 for Fortran-style)</param>
    /// <param name="eval_f">Callback for objective function evaluation</param>
    /// <param name="eval_g">Callback for constraint function evaluation</param>
    /// <param name="eval_grad_f">Callback for objective gradient evaluation</param>
    /// <param name="eval_jac_g">Callback for constraint Jacobian evaluation</param>
    /// <param name="eval_h">Callback for Hessian evaluation</param>
    /// <returns>Handle to the created IPOPT problem, or null on failure</returns>
    [<DllImport("ipopt", CallingConvention = CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
    extern nativeint CreateIpoptProblem(
        int n,
        nativeint x_L,
        nativeint x_U,
        int m,
        nativeint g_L,
        nativeint g_U,
        int nele_jac,
        int nele_hess,
        int index_style,
        EvalF eval_f,
        EvalG eval_g,
        EvalGradF eval_grad_f,
        EvalJacG eval_jac_g,
        EvalH eval_h)

    /// <summary>
    /// Frees an IPOPT problem instance and releases associated resources.
    /// </summary>
    /// <param name="ipopt_problem">Handle to the IPOPT problem to free</param>
    [<DllImport("ipopt", CallingConvention = CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
    extern void FreeIpoptProblem(nativeint ipopt_problem)

    /// <summary>
    /// Sets a string option for the IPOPT problem.
    /// </summary>
    /// <param name="ipopt_problem">Handle to the IPOPT problem</param>
    /// <param name="keyword">Option name</param>
    /// <param name="value">Option value</param>
    /// <returns>1 on success, 0 on failure</returns>
    [<DllImport("ipopt", CallingConvention = CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
    extern int AddIpoptStrOption(nativeint ipopt_problem, string keyword, string value)

    /// <summary>
    /// Sets a numeric (double) option for the IPOPT problem.
    /// </summary>
    /// <param name="ipopt_problem">Handle to the IPOPT problem</param>
    /// <param name="keyword">Option name</param>
    /// <param name="value">Option value</param>
    /// <returns>1 on success, 0 on failure</returns>
    [<DllImport("ipopt", CallingConvention = CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
    extern int AddIpoptNumOption(nativeint ipopt_problem, string keyword, double value)

    /// <summary>
    /// Sets an integer option for the IPOPT problem.
    /// </summary>
    /// <param name="ipopt_problem">Handle to the IPOPT problem</param>
    /// <param name="keyword">Option name</param>
    /// <param name="value">Option value</param>
    /// <returns>1 on success, 0 on failure</returns>
    [<DllImport("ipopt", CallingConvention = CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
    extern int AddIpoptIntOption(nativeint ipopt_problem, string keyword, int value)

    /// <summary>
    /// Sets the callback invoked once per iteration; returning false from it stops the solver.
    /// </summary>
    /// <param name="ipopt_problem">Handle to the IPOPT problem</param>
    /// <param name="intermediate_cb">The callback (must be kept alive until IpoptSolve returns)</param>
    /// <returns>True on success</returns>
    [<DllImport("ipopt", CallingConvention = CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
    extern [<MarshalAs(UnmanagedType.U1)>] bool SetIntermediateCallback(nativeint ipopt_problem, Intermediate intermediate_cb)

    /// <summary>
    /// Reads the current iterate; only valid while the intermediate callback runs.
    /// Pointers may be null for the values that are not needed.
    /// </summary>
    /// <param name="ipopt_problem">Handle to the IPOPT problem</param>
    /// <param name="scaled">Whether to return the scaled (internal) or the unscaled values</param>
    /// <param name="n">Number of variables</param>
    /// <param name="x">Buffer for the variable values (n entries)</param>
    /// <param name="z_L">Buffer for the lower bound multipliers (n entries)</param>
    /// <param name="z_U">Buffer for the upper bound multipliers (n entries)</param>
    /// <param name="m">Number of constraints</param>
    /// <param name="g">Buffer for the constraint values (m entries)</param>
    /// <param name="lambda">Buffer for the constraint multipliers (m entries)</param>
    /// <returns>True on success</returns>
    [<DllImport("ipopt", CallingConvention = CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
    extern [<MarshalAs(UnmanagedType.U1)>] bool GetIpoptCurrentIterate(
        nativeint ipopt_problem,
        [<MarshalAs(UnmanagedType.U1)>] bool scaled,
        int n,
        nativeint x,
        nativeint z_L,
        nativeint z_U,
        int m,
        nativeint g,
        nativeint lambda)

    /// <summary>
    /// Solves the optimization problem.
    /// </summary>
    /// <param name="ipopt_problem">Handle to the IPOPT problem</param>
    /// <param name="x">Pointer to initial values and will contain solution on return</param>
    /// <param name="g">Pointer to constraint values at solution</param>
    /// <param name="obj_val">Pointer to store the final objective value</param>
    /// <param name="mult_g">Pointer to constraint multipliers at solution</param>
    /// <param name="mult_x_L">Pointer to lower bound multipliers at solution</param>
    /// <param name="mult_x_U">Pointer to upper bound multipliers at solution</param>
    /// <param name="user_data">Pointer to user data (unused)</param>
    /// <returns>Status code indicating the solution status</returns>
    [<DllImport("ipopt", CallingConvention = CallingConvention.Cdecl); SuppressUnmanagedCodeSecurity>]
    extern int IpoptSolve(
        nativeint ipopt_problem,
        nativeint x,
        nativeint g,
        nativeint obj_val,
        nativeint mult_g,
        nativeint mult_x_L,
        nativeint mult_x_U,
        nativeint user_data)
