/*
 * @Author: Chao Yang
 * @Date: 2020-11-18 10:36:09
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-11-18 11:12:19
 */

using System;

namespace Chao.Abp.ResultHandling.Model;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
public class WrapResultAttribute : Attribute
{
    public WrapResultAttribute(bool wrapOnSuccess = true, bool wrapOnError = true)
    {
        WrapOnSuccess = wrapOnSuccess;
        WrapOnError = wrapOnError;
    }

    public bool WrapOnError { get; set; }
    public bool WrapOnSuccess { get; set; }
}