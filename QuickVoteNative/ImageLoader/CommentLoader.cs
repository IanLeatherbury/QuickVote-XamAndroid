﻿/*
 * Copyright (C) 2013 @JamesMontemagno http://www.montemagno.com http://www.refractored.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Ported from Xamarin Sample App
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Widget;
using Java.IO;
using Java.Net;
using File = Java.IO.File;
using Android.Graphics.Drawables;
using QuickVoteNative.Imageloaders;

namespace QuickVoteNative.Commentloaders
{

    public class CommentLoader
    {
        public static void CopyStream(Stream inputStream, OutputStream os)
        {
            int buffer_size=1024;
            try
            {
                byte[] bytes=new byte[buffer_size];
                for(;;)
                {
                    int count = inputStream.Read(bytes, 0, buffer_size);
                    if(count <= 0)
                        break;
                    os.Write(bytes, 0, count);
                }
            }
            catch(Exception ex){
                System.Console.WriteLine("Copy stream exception: " + ex);
            }
        }

        internal class PhotoToLoad
        {
            public String Url;
            public ImageView ImageView;
            public PhotoToLoad(String url, ImageView imageView)
            {
                Url = url;
                ImageView = imageView;
            }
        }

        private MemoryCache m_MemoryCache = new MemoryCache();
        private FileCache m_FileCache;

        private IDictionary<ImageView, String> m_ImageViews = new ConcurrentDictionary<ImageView, String>();
        private int m_StubID = -1;

        private int m_Scale;
        private int m_MaxComments;

        public CommentLoader(Context context, int scale = 64, int maxComments = 0)
        {
            m_FileCache = new FileCache(context);
            m_Scale = scale;
            m_MaxComments = maxComments;
        }

        public void DisplayComment(string url, ImageView imageView, int defaultResourceId)
        {
            m_StubID = defaultResourceId;
            if (m_ImageViews.ContainsKey(imageView))
            {
                if(defaultResourceId != -1)
                    imageView.SetImageResource(defaultResourceId);

                m_ImageViews.Remove(imageView);
            }

            m_MemoryCache.PopCache(m_MaxComments);

            m_ImageViews.Add(imageView, url);

            var bitmap = m_MemoryCache.Get(url);
            if (bitmap != null)
            {
                imageView.SetImageDrawable(new CircleDrawable(bitmap));//SetCommentBitmap(bitmap);
            }
            else
            {
                QueueComment(url, imageView);
                if(defaultResourceId != -1)
                    imageView.SetImageResource(defaultResourceId);
            }
        }

        public void QueueComment(string url, ImageView CommentView)
        {
            var photoToUpload = new PhotoToLoad(url, CommentView);
            ThreadPool.QueueUserWorkItem(state => LoadPhoto(photoToUpload));
        }

        private Bitmap GetBitmap(string url)
        {
            File f= m_FileCache.GetFile(url);

            ////from SD cache
            Bitmap b = DecodeFile(f, m_Scale);
            if(b!=null)
                return b;

            ////from web
            try
            {
                Bitmap bitmap=null;
                URL CommentUrl = new URL(url);
                HttpURLConnection conn = (HttpURLConnection)CommentUrl.OpenConnection();
                conn.ConnectTimeout = 5000;
                conn.ReadTimeout = 5000;
                conn.InstanceFollowRedirects = true;

                if (conn.ErrorStream != null)
                    return null;

                var inputStream = conn.InputStream;
                OutputStream os = new FileOutputStream(f);
                CopyStream(inputStream, os);
                os.Close();
                bitmap = DecodeFile(f, m_Scale);
                return bitmap;
            } 
            catch (Exception ex)
            {
                //ex.printStackTrace();
                return null;
            }
        }

        private static Bitmap DecodeFile(File file, int requiredSize)
        {
            try
            {
                //decode Comment size
                BitmapFactory.Options options = new BitmapFactory.Options {InJustDecodeBounds = true, InPurgeable = true};

                BitmapFactory.DecodeStream(new FileStream(file.Path, FileMode.OpenOrCreate, FileAccess.ReadWrite,FileShare.ReadWrite), null, options);//FileStream?

                //Find the correct scale value. It should be the power of 2.
                int tempWidth = options.OutWidth;
                int tempHeight = options.OutHeight;

                var scale = 1;

                while(true)
                {
                    if (tempWidth / 2 < requiredSize || tempHeight / 2 < requiredSize)
                        break;

                    tempWidth/=2;
                    tempHeight/=2;
                    scale*=2;
                }

                //decode with inSampleSize
                BitmapFactory.Options options2 = new BitmapFactory.Options {InSampleSize = scale};

                return BitmapFactory.DecodeStream(new FileStream(file.Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite), null, options2);//FileStream?
            } 
            catch(Exception e)
            {

            }

            return null;
        }

        internal bool CommentViewReused(PhotoToLoad photoToLoad)
        {
            try
            {
                if (!m_ImageViews.ContainsKey(photoToLoad.ImageView))
                    return true;

                if (!m_ImageViews[photoToLoad.ImageView].Equals(photoToLoad.Url))
                    return true;
            }
            catch (Exception)
            {
            }

            return false;
        }

        public void LoadPhoto(object param)
        {
            var photoToLoad = param as PhotoToLoad;
            if (photoToLoad == null)
                return;

            if (CommentViewReused(photoToLoad))
                return;

            Bitmap bitmap = GetBitmap(photoToLoad.Url);
            m_MemoryCache.Put(photoToLoad.Url, bitmap);
            if (CommentViewReused(photoToLoad))
                return;

            BitmapDisplayer(bitmap, photoToLoad);
        }

        internal void BitmapDisplayer(Bitmap bitmap, PhotoToLoad photoToLoad)
        {
            var activity = (Activity) photoToLoad.ImageView.Context;
            activity.RunOnUiThread(() =>
                {
                    if (CommentViewReused(photoToLoad))
                        return;
                    photoToLoad.ImageView.Visibility = Android.Views.ViewStates.Visible;
                    if (bitmap != null)
                        photoToLoad.ImageView.SetImageDrawable(new CircleDrawable(bitmap));//SetCommentBitmap(bitmap);
                    else if(m_StubID != -1)
                        photoToLoad.ImageView.SetImageResource(m_StubID);
                });
        }

        public void ClearCache()
        {
            m_MemoryCache.Clear();
            //m_FileCache.Clear();
            m_ImageViews.Clear();
        }

        public void ClearFileCache()
        {
            m_FileCache.Clear();
        }

        public class CircleDrawable : Drawable
        {
            Bitmap bmp;
            BitmapShader bmpShader;
            Paint paint;
            RectF oval;

            public CircleDrawable(Bitmap bmp)
            {
                this.bmp = bmp;
                this.bmpShader = new BitmapShader(bmp, Shader.TileMode.Clamp, Shader.TileMode.Clamp);
                this.paint = new Paint() { AntiAlias = true };
                this.paint.SetShader(bmpShader);
                this.oval = new RectF();
            }

            public override void Draw(Canvas canvas)
            {
                canvas.DrawOval(oval, paint);
            }

            protected override void OnBoundsChange(Rect bounds)
            {
                base.OnBoundsChange(bounds);
                oval.Set(0, 0, bounds.Width(), bounds.Height());
            }

            public override int IntrinsicWidth
            {
                get
                {
                    return bmp.Width;
                }
            }

            public override int IntrinsicHeight
            {
                get
                {
                    return bmp.Height;
                }
            }

            public override void SetAlpha(int alpha)
            {

            }

            public override int Opacity
            {
                get
                {
                    return (int)Format.Opaque;
                }
            }

            public override void SetColorFilter(ColorFilter cf)
            {

            }
        }
    }
}