PixiScript is a language for writing PixiEditor Nodes.

Each node consists of a set of inputs, outputs and execution logic.

A node supports CPU and GPU executin logic. PixiScript compiles into host native code and GPU shaders.

## The goal

PixiScript aims to run PixiEditor Node Graphs outside of PixiEditor.

## PixiScript Example Node

```
input num someNum;
output painter result;

num scale;

// on node execute
void execute()
{
num x = 4;
num y = 10;
scale = x + y;
}

// result draw func
result_paint(gpu_ctx)
{
gpu_ctx.drawCircle(0, 0, scale, scale);
}
```